using System.ComponentModel.DataAnnotations;

namespace ManejoPresupuestos.Models.Validaciones
{
    public class PrimeraLetraMayusculaAtribute : ValidationAttribute
    {
        //Sobreescribimos el metodo validationResult
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            //Validamos que el campo no este vacio
            if (value == null || string.IsNullOrEmpty(value.ToString()))
            {
                return ValidationResult.Success;
            }
            //Vamos a obtener la primera letra de nuestra cadena
            var primeraLetra = value.ToString()[0].ToString();
            //Validamos que si la primera letra es distinto a la primera letra en mayusculas me envie un mensaje
            if (primeraLetra != primeraLetra.ToUpper())
            {
                //Retornamos un error
                return new ValidationResult("La primera letra debe estar en mayusculas");
            }
            //Retornamos un mensaje de validacion que sea correcta
            return ValidationResult.Success;
        }
    }
}
