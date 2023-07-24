using Dapper;
using ManejoPresupuestos.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuestos.Servicios
{
    //vamos a crear una intefaz para poder conectarnos a nuestra tablas
    public interface IRepositorioTipoCuenta
    {
        Task Actualizar(TipoCuenta tipoCuenta);
        Task Borrar(int id);
        Task Crear(TipoCuenta tipoCuenta);
        Task<bool> Existe(string nombre, int usuarioId);
        Task<IEnumerable<TipoCuenta>> Obtener(int usuarioId);
        Task<TipoCuenta> ObtenerPorId(int id, int usuarioId);
    }
    public class RepositorioTipoCuenta : IRepositorioTipoCuenta
    {
        private readonly string connectingString;

        //Creamos un metodo para la conexion

        public RepositorioTipoCuenta(IConfiguration configuration)
        {
            connectingString = configuration.GetConnectionString("DefaultConnection");
        }
        public async Task Crear(TipoCuenta tipoCuenta)
        {
            using var connection = new SqlConnection(connectingString);
            var id = await connection.QuerySingleAsync<int>($@"INSERT INTO TiposCuentas(Nombre, UsuarioId, Orden)
                                                 Values(@Nombre, @UsuarioId,0);
                                                 SELECT SCOPE_IDENTITY();", tipoCuenta);
            tipoCuenta.Id = id;
        }
        //Creamos un metodo para validar la informacion que estamos ingresando
        //y que no se repita en nuestra base de datos
        public async Task<bool> Existe(string nombre, int usuarioId)
        {
            //Nos conectamos a la BBD mediante la cadena de conexion
            using var connection = new SqlConnection(connectingString);
            var existe = await connection.QueryFirstOrDefaultAsync<int>(
                @"SELECT 1 FROM TiposCuentas WHERE
                @Nombre=nombre AND @usuarioId=usuarioId;", new { nombre, usuarioId });

            return existe == 1;
        }
        //Creamos un metodo para listar los datos de la table TiposCuenta

        public async Task<IEnumerable<TipoCuenta>> Obtener(int usuarioId)
        {
            using var connection = new SqlConnection(connectingString);
            return await connection.QueryAsync<TipoCuenta>(@"SELECT Id, Nombre, Orden
                                                           FROM TiposCuentas WHERE UsuarioId=@UsuarioId",
                                                           new { usuarioId });
        }
        //Creamos un metodo para poder actualizar un registro en el campo de nuestra tabla tiposCuentas
        public async Task Actualizar(TipoCuenta tipoCuenta)
        {
            using var connection = new SqlConnection(connectingString);
            await connection.ExecuteAsync(@"UPDATE TiposCuentas SET Nombre=@Nombre
                                          WHERE Id=@Id", tipoCuenta);
        }
        //Creamos el metodo para obtener el id
        public async Task<TipoCuenta> ObtenerPorId(int id, int usuarioId)
        {
            using var connection = new SqlConnection(connectingString);
            return await connection.QueryFirstOrDefaultAsync<TipoCuenta>(@"SELECT Id, Nombre, Orden
                                                                        FROM TiposCuentas
                                                                        WHERE Id=@Id AND UsuarioId=@UsuarioId",
                                                                        new { id, usuarioId });
        }
        //Creamos un metodo para borrar un registro
        public async Task Borrar(int id)
        {
            using var connection = new SqlConnection(connectingString);
            await connection.ExecuteAsync("DELETE TiposCuentas WHERE Id = @Id", new { id });
        }
    }
}
