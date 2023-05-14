using Microsoft.EntityFrameworkCore;
using ApiVetPet.Data;
using System.Data;
using Microsoft.Data.SqlClient;
using NugetVetPet.Models;
using ApiVetPet.Helpers;

#region TABLES

//create table SERVICIOS (
//IDSERVICIO int primary key,
//NOMBRE nvarchar(50),
//DESCRIPCION nvarchar(500),
//)

//create table VACUNAS(
//  IDVACUNA int primary key, 
//  IDUSUARIO int, 
//  IDMASCOTA int, 
//  NVACUNA NVARCHAR(50),
//  LOTE NVARCHAR(50), 
//    FECHA DATE
//)

//create table MASCOTAS(
//	IDMASCOTA int primary key,
//    NMASCOTA nvarchar(50),
//	EDAD int,
//	PESO int,
//	RAZA nvarchar(50),
//	IDUSUARIO int
//)

//create table CITAS(
//	IDCITA int primary key,
//    IDUSUARIO int,
//    IDMASCOTA int,
//    TIPO_CITA NVARCHAR(50),
//	DIA_CITA DATETIME
//)

//CREATE TABLE PRUEBAS(
//IDPRUEBA INT,
//IDUSUARIO INT,
//IDMASCOTA INT,
//NAME_FILE NVARCHAR(MAX),
//DESCRIPCION NVARCHAR(150),
//FECHA DATE
//)

//CREATE TABLE TRATAMIENTOS(
//IDTRATAMIENTO INT PRIMARY KEY,
//IDUSUARIO INT,
//IDMASCOTA INT,
//NOMBREMEDICACION NVARCHAR(75),
//DOSIS NVARCHAR(20),
//DURACION NVARCHAR(50),
//DESCRIPCION NVARCHAR(100)
//)

//create TABLE USUARIOS (
//	IDUSUARIO INT PRIMARY KEY,
//    APODO NVARCHAR(50) NOT NULL,
//    NOMBRE NVARCHAR(50),
//    TELEFONO NVARCHAR(15),
//	  EMAIL NVARCHAR(150) UNIQUE NOT NULL,
//    SALT NVARCHAR(MAX) NOT NULL,
//    PASS NVARCHAR(50) NOT NULL,
//    PASS_CIFRADA NVARCHAR(MAX) NOT NULL,
//    IMAGEN NVARCHAR(MAX) DEFAULT 'default_image.jpg'
//)

#endregion

#region VISTAS

//CREATE VIEW V_TRATAMIENTOS
//AS
//	SELECT IDTRATAMIENTO, TRATAMIENTOS.IDUSUARIO, TRATAMIENTOS.IDMASCOTA, NMASCOTA, NOMBREMEDICACION, DOSIS, DURACION, DESCRIPCION
//	FROM TRATAMIENTOS
//	LEFT JOIN MASCOTAS
//	ON TRATAMIENTOS.IDMASCOTA = MASCOTAS.IDMASCOTA
//GO

//CREATE VIEW V_PRUEBAS
//AS
//	SELECT IDPRUEBA, PRUEBAS.IDUSUARIO, PRUEBAS.IDMASCOTA, NMASCOTA, NAME_FILE, DESCRIPCION, FECHA
//	FROM PRUEBAS
//	LEFT JOIN MASCOTAS
//	ON PRUEBAS.IDMASCOTA = MASCOTAS.IDMASCOTA
//GO

//create VIEW V_VACUNAS
//as
//	select IDVACUNA, vacunas.IDUSUARIO, vacunas.IDMASCOTA, NMASCOTA, NVACUNA, LOTE, FECHA, mascotas.IMAGEN
//	from vacunas 
//	left join mascotas
//	on vacunas.idmascota = mascotas.idmascota
//go

#endregion

#region PROCEDURES

//CREATE PROCEDURE SP_VACUNAS_PAGINAR
//(@POSICION INT, @IDUSUARIO INT)
//AS
//    SELECT POSICION, IDVACUNA, IDUSUARIO, IDMASCOTA, NMASCOTA, NVACUNA, LOTE, FECHA, IMAGEN FROM
//        (SELECT CAST(
//            ROW_NUMBER() OVER(ORDER BY FECHA DESC) AS INT) AS POSICION,
//            IDVACUNA, IDUSUARIO, IDMASCOTA, NMASCOTA, NVACUNA, LOTE, FECHA, IMAGEN
//        FROM V_VACUNAS
//        WHERE IDUSUARIO = @IDUSUARIO) AS QUERY
//    WHERE QUERY.POSICION >= @POSICION AND QUERY.POSICION < (@POSICION + 5)
//GO

#endregion

namespace ApiVetPet.Repositories
{
    public class RepositoryUsuarios
    {
        private UsuariosContext context;

        public RepositoryUsuarios(UsuariosContext context)
        {
            this.context = context;
        }


        #region Controller OAuth

        public async Task<Usuario> ExisteUsuario(string username, string password)
        {

            Usuario user = new Usuario();

            if (username.IndexOf("@") != -1)
            {
                user = await
                this.context.Usuarios.FirstOrDefaultAsync(x => x.Email == username);

            }
            else
            {
                user = await
                this.context.Usuarios.FirstOrDefaultAsync(x => x.Apodo == username);
            }


            if (user == null)
            {
                return null;
            }
            else
            {
                //RECUPERAMOS EL PASSWORD CIFRADO DE LA BBDD
                byte[] passUsuario = user.Password;
                //DEBEMOS CIFRAR DE NUEVO EL PASSWORD DE USUARIO
                //JUNTO A SU SALT UTILIZANDO LA MISMA TECNICA
                string salt = user.Salt;
                byte[] temp =
                    HelperCryptography.EncryptPassword(password, salt);

                //COMPARAMOS LOS DOS ARRAYS
                bool respuesta =
                    HelperCryptography.CompareArrays(passUsuario, temp);
                if (respuesta == true)
                {
                    //SON IGUALES
                    return user;
                }
                else
                {
                    return null;
                }
            }

        }

        private async Task<int> GetMaxUserAsync()
        {
            if (this.context.Usuarios.Count() == 0)
            {
                return 1;
            }
            else
            {
                return await this.context.Usuarios.MaxAsync(x => x.IdUsuario) + 1;
            }
        }

        public async Task RegisterAsync(Usuario user)
        {
            var Salt =
               HelperCryptography.GenerateSalt();

            Usuario newUser = new Usuario()
            {
                IdUsuario = await this.GetMaxUserAsync(),
                Apodo = user.Apodo,
                Nombre = user.Nombre,
                Email = user.Email,
                Telefono = user.Telefono,
                Pass = user.Pass,
                Salt = Salt,
                Password = HelperCryptography.EncryptPassword(user.Pass, Salt),
                Imagen = user.Imagen
            };

            this.context.Add(newUser);
            await this.context.SaveChangesAsync();
        }

        #endregion


        #region FORMS

        public async Task CreateCita(Cita cita, int idUsuario)
        {
            Cita newCita = new Cita()
            {
                IdCita = await this.GetMaxIdCita(),
                TipoCita = cita.TipoCita,
                IdMascota = cita.IdMascota,
                IdUsuario = cita.IdUsuario,
                DiaCita = cita.DiaCita
            };

            this.context.Citas.Add(cita);
            await this.context.SaveChangesAsync();
        }

        public async Task<Usuario> UpdateUsuario(Usuario usuario)
        {
            Usuario user = await FindUserAsync(usuario.IdUsuario);
            user.Nombre = usuario.Nombre;
            user.Apodo = usuario.Apodo;
            user.Email = usuario.Email;
            user.Telefono = usuario.Telefono;
            user.Imagen = usuario.Imagen;

            this.context.Usuarios.Update(user);
            await this.context.SaveChangesAsync();
            return user;
        }


        public async Task<Mascota> UpdateMascota(Mascota mascota)
        {

            Mascota pet = await FindPetAsync(mascota.IdMascota);
            pet.Nombre = mascota.Nombre;
            pet.Raza = mascota.Raza;
            pet.Tipo = mascota.Tipo;
            pet.Peso = mascota.Peso;
            pet.Fecha_Nacimiento = mascota.Fecha_Nacimiento;
            pet.Imagen = mascota.Imagen;

            this.context.Mascotas.Update(pet);
            await this.context.SaveChangesAsync();
            return mascota;
        }


        #endregion


        #region FINDS

        public async Task<Usuario> FindUserAsync(int idusuario)
        {
            return await
                this.context.Usuarios
                .FirstOrDefaultAsync(x => x.IdUsuario == idusuario);
        }

        public async Task<Mascota> FindPetAsync(int idmascota)
        {
            return await
                this.context.Mascotas
                .FirstOrDefaultAsync(x => x.IdMascota == idmascota);
        }

        #endregion


        #region GETS

        private async Task<int> GetMaxIdCita()
        {
            if (this.context.Citas.Count() == 0)
            {
                return 1;
            }
            else
            {
                return this.context.Citas.Max(z => z.IdCita) + 1;
            }
        }


        public async Task<List<Mascota>> GetMascotas(int idusuario)
        {
            List<Mascota> mascotas = this.context.Mascotas.Where(x => x.IdUsuario == idusuario).ToList();
            return mascotas;
        }

        public async Task<List<Tratamiento>> GetTratamientos(int idusuario)
        {
            List<Tratamiento> tratamientos = this.context.Tratamientos.Where(x => x.IdUsuario == idusuario).ToList();
            return tratamientos;
        }

        public async Task<List<Vacuna>> GetVacunas(int idusuario)
        {
            List<Vacuna> vacunas = this.context.Vacunas.Where(x => x.IdUsuario == idusuario).OrderByDescending(x => x.Fecha).ToList();
            return vacunas;
        }

        public async Task<List<Cita>> GetCitas()
        {
            List<Cita> citas = this.context.Citas.ToList();
            return citas;
        }

        public async Task<List<Evento>> GetEventos(int idusuario)
        {
            List<Evento> eventos = this.context.Eventos.Where(x => x.resourceid == idusuario).ToList();
            return eventos;
        }

        public async Task<List<Prueba>> GetPruebas(int idusuario)
        {
            List<Prueba> pruebas = this.context.Pruebas.Where(x => x.IdUsuario == idusuario).OrderByDescending(x => x.Fecha).ToList();
            return pruebas;
        }


        public async Task<List<Servicio>> GetServicios()
        {
            List<Servicio> servicios = this.context.Servicios.ToList();
            return servicios;
        }

        public async Task<List<FAQ>> GetFAQs()
        {
            List<FAQ> faqs = this.context.FAQs.ToList();
            return faqs;
        }

        #endregion

    }
}