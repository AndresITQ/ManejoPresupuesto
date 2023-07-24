namespace ManejoPresupuestos.Servicios
{

    //creamos la interfaz
    public interface IServicioUsuario
    {
        int ObtenerUsuarioId();
    }
    public class ServiciosUsuarios : IServicioUsuario
    {
        //Creamos un metodo para obtener el idUsuario
        public int ObtenerUsuarioId()
        {
            return 1;
        }
    }
}
