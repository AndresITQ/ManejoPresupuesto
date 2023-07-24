using Dapper;
using ManejoPresupuestos.Models;
using ManejoPresupuestos.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuestos.Controllers
{
    public class TiposCuentasController : Controller
    {
        private readonly string connectionString;
        private readonly IRepositorioTipoCuenta repositorioTiposCuentas;
        private readonly IServicioUsuario servicioUsuario;
        public TiposCuentasController(IRepositorioTipoCuenta repositorioTiposCuentas, IServicioUsuario servicioUsuario)
        {
            this.repositorioTiposCuentas = repositorioTiposCuentas;
            this.servicioUsuario = servicioUsuario;
        }
        //Creamos una accion para listar los tipos de cuenta
        public async Task<IActionResult> Index()
        {
            var usuarioId = 1;
            var tiposCuentas = await repositorioTiposCuentas.Obtener(usuarioId);
            return View(tiposCuentas);
        }
        public IActionResult Crear()
        {
            //abrir la conexion a la base de datos
            /*using (var connection = new SqlConnection(connectionString))
            {
                var query = connection.Query("SELECT 1").FirstOrDefault();
            }*/
            return View();
        }

        //Creamos una acción para enviar los datos por el método POST
        [HttpPost]
        public async Task<IActionResult> Crear(TipoCuenta tipoCuenta)
        {
            //Creamos una condicion de evaluacion para saber si el campo que estamos enviando
            //datos validos
            if (!ModelState.IsValid)
            {
                return View(tipoCuenta);
            }
            //llamamos al metodo crear e ingresar el usuarioId
            tipoCuenta.UsuarioId = servicioUsuario.ObtenerUsuarioId();
            //Creamos una variable donde vamos a guardar las respuesta
            var yaExisteTipoCuenta = await repositorioTiposCuentas.Existe(tipoCuenta.Nombre, tipoCuenta.UsuarioId);
            //Creamos una condicion para verificar si el dato ingresado ya existe
            if (yaExisteTipoCuenta)
            {
                ModelState.AddModelError(nameof(tipoCuenta.Nombre), $"El nombre {tipoCuenta.Nombre} ya existe");
                return View(tipoCuenta);
            }
            await repositorioTiposCuentas.Crear(tipoCuenta);

            return RedirectToAction("Index");
        }
        //Creamos un metodo para poder validar el campo que voy a ingresar
        [HttpGet]
        public async Task<IActionResult> VerificarExisteTipoCuenta(string nombre)
        {
            var usuarioId = 1;
            var yaExisteTipoCuenta = await repositorioTiposCuentas.Existe(nombre, usuarioId);
            if (yaExisteTipoCuenta)
            {
                return Json($"El nombre {nombre} ya existe ");
            }
            return Json(true);
        }
        //Este metodo me va a permitir cargar los datos del registro por su id
        [HttpGet]
        public async Task<ActionResult> Editar(int id)
        {
            var usuarioId = servicioUsuario.ObtenerUsuarioId();
            var tipoCuenta = await repositorioTiposCuentas.ObtenerPorId(id, usuarioId);
            if (tipoCuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            return View(tipoCuenta);
        }
        //Creamos un metodo para editar los registros
        [HttpPost]
        public async Task<ActionResult> Editar(TipoCuenta tipoCuenta)
        {
            //Obtenemos el id del usuario obtengo este id
            //por que no puedo decir envie otro id que no sea su propio
            //id de esa manera no puede hacerse pasar como un administrador
            var usuarioId = servicioUsuario.ObtenerUsuarioId();
            var tipoCuentaExiste = await repositorioTiposCuentas.ObtenerPorId(tipoCuenta.Id, usuarioId);

            if (tipoCuentaExiste is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            await repositorioTiposCuentas.Actualizar(tipoCuenta);
            return RedirectToAction("Index");
        }
        //Creamos una accion para borrar un registro
        public async Task<IActionResult> Borrar(int id)
        {
            //Obtenemos el id del usuario
            var usuarioId = servicioUsuario.ObtenerUsuarioId();
            var tipoCuenta = await repositorioTiposCuentas.ObtenerPorId(id, usuarioId);
            if (tipoCuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            return View(tipoCuenta);
        }
        //Creamos el metodo post para le borrado de datos
        [HttpPost]
        public async Task<IActionResult> BorrarTipoCuenta(int id)
        {
            //Obtenemos el id del usuario
            var usuarioId = servicioUsuario.ObtenerUsuarioId();
            var tipoCuenta = await repositorioTiposCuentas.ObtenerPorId(id, usuarioId);
            if (tipoCuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            await repositorioTiposCuentas.Borrar(id);
            return RedirectToAction("Index");
        }
    }
}
