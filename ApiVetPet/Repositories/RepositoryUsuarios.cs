using Microsoft.EntityFrameworkCore;
using ApiVetPet.Data;
using System.Data;
using Microsoft.Data.SqlClient;
using NugetVetPet.Models;

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


        #region FORMS

        public async Task CreateCita(int idusuario, int idmascota, string tipo, DateTime fecha)
        {
            Cita cita = new Cita();
            cita.IdCita = this.GetMaxIdCita();
            cita.TipoCita = tipo;
            cita.IdMascota = idmascota;
            cita.IdUsuario = idusuario;
            cita.DiaCita = fecha;

            this.context.Citas.Add(cita);
            await this.context.SaveChangesAsync();
        }

        public async Task<Usuario> UpdateUsuario(int idusuario, string nombre, string apodo,
            string email, string telefono)
        {
            Usuario user = await FindUserAsync(idusuario);
            user.Nombre = nombre;
            user.Apodo = apodo;
            user.Email = email;
            user.Telefono = telefono;

            this.context.Usuarios.Update(user);
            await this.context.SaveChangesAsync();
            return user;
        }

        public async Task<Usuario> UpdateUsuario(int idusuario, string nombre, string apodo,
            string email, string telefono, string fileName)
        {
            Usuario user = await FindUserAsync(idusuario);
            user.Nombre = nombre;
            user.Apodo = apodo;
            user.Email = email;
            user.Telefono = telefono;
            user.Imagen = fileName;

            this.context.Usuarios.Update(user);
            await this.context.SaveChangesAsync();
            return user;
        }


        public async Task<Mascota> UpdateMascota(int idusuario, int idmascota, string nombre, string raza,
            string tipo, int peso, DateTime fechanacimiento)
        {
            Mascota mascota = await FindPetAsync(idmascota);
            mascota.Nombre = nombre;
            mascota.Raza = raza;
            mascota.Tipo = tipo;
            mascota.Peso = peso;
            mascota.Fecha_Nacimiento = fechanacimiento;

            this.context.Mascotas.Update(mascota);
            await this.context.SaveChangesAsync();
            return mascota;
        }

        public async Task<Mascota> UpdateMascota(int idusuario, int idmascota, string nombre, string raza,
            string tipo, int peso, DateTime fechanacimiento, string fileName)
        {
            Mascota mascota = await FindPetAsync(idmascota);
            mascota.Nombre = nombre;
            mascota.Raza = raza;
            mascota.Tipo = tipo;
            mascota.Peso = peso;
            mascota.Fecha_Nacimiento = fechanacimiento;
            mascota.Imagen = fileName;

            this.context.Mascotas.Update(mascota);
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

        private int GetMaxIdUsuario()
        {
            if (this.context.Usuarios.Count() == 0)
            {
                return 1;
            }
            else
            {
                return this.context.Usuarios.Max(z => z.IdUsuario) + 1;
            }
        }

        private int GetMaxIdCita()
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


        public int GetNumeroVacunas(int idusuario)
        {
            return this.context.Vacunas.
                Where(z => z.IdUsuario == idusuario).Count();
        }

        public int GetNumeroProcedimientos(int idusuario)
        {
            return this.context.Procedimientos.
                Where(z => z.IdUsuario == idusuario).Count();
        }


        public List<Mascota> GetMascotas(int idusuario)
        {
            List<Mascota> mascotas = this.context.Mascotas.Where(x => x.IdUsuario == idusuario).ToList();
            return mascotas;
        }

        public List<Tratamiento> GetTratamientos(int idusuario)
        {
            List<Tratamiento> tratamientos = this.context.Tratamientos.Where(x => x.IdUsuario == idusuario).ToList();
            return tratamientos;
        }

        public List<Vacuna> GetVacunas(int idusuario)
        {
            List<Vacuna> vacunas = this.context.Vacunas.Where(x => x.IdUsuario == idusuario).OrderByDescending(x => x.Fecha).ToList();
            return vacunas;
        }

        public List<Cita> GetCitas()
        {
            List<Cita> citas = this.context.Citas.ToList();
            return citas;
        }

        public List<Evento> GetEventos(int idusuario)
        {
            List<Evento> eventos = this.context.Eventos.Where(x => x.resourceid == idusuario).ToList();
            return eventos;
        }

        public List<Prueba> GetPruebas(int idusuario)
        {
            List<Prueba> pruebas = this.context.Pruebas.Where(x => x.IdUsuario == idusuario).OrderByDescending(x => x.Fecha).ToList();
            return pruebas;
        }


        public async Task<List<Vacuna>>
        GetVacunasPaginar(int posicion, int idusuario)
        {
            string sql =
                "SP_VACUNAS_PAGINAR @POSICION, @IDUSUARIO";
            SqlParameter pamposicion =
                new SqlParameter("@POSICION", posicion);
            SqlParameter pamidusuario =
                new SqlParameter("@IDUSUARIO", idusuario);

            var consulta =
                this.context.Vacunas.FromSqlRaw(sql, pamposicion, pamidusuario);
            List<Vacuna> vacunas = await consulta.ToListAsync();

            return vacunas;
        }

        public async Task<List<Procedimiento>>
        GetProcedimientosPaginar(int posicion, int idusuario)
        {
            string sql =
                "SP_PROCEDIMIENTOS_PAGINAR @POSICION, @IDUSUARIO";
            SqlParameter pamposicion =
                new SqlParameter("@POSICION", posicion);
            SqlParameter pamidusuario =
                new SqlParameter("@IDUSUARIO", idusuario);

            var consulta =
                this.context.Procedimientos.FromSqlRaw(sql, pamposicion, pamidusuario);
            List<Procedimiento> procedimientos = await consulta.ToListAsync();

            return procedimientos;
        }


        public List<Servicio> GetServicios()
        {
            List<Servicio> servicios = this.context.Servicios.ToList();
            return servicios;
        }

        public List<FAQ> GetFAQs()
        {
            List<FAQ> faqs = this.context.FAQs.ToList();
            return faqs;
        }

        #endregion

    }
}