using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using ProyectoIdentity.Models;
using ProyectoIdentity.Models.ViewModels;
using System.Security.Claims;

namespace ProyectoIdentity.Controllers
{
    public class CuentasController : Controller
    {
        #region CONSTRUCTOR
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IEmailSender _emailSender;
        public CuentasController(UserManager<IdentityUser> userManager,
                                 SignInManager<IdentityUser> signInManager,
                                 IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }

        #endregion

        #region REGISTRO
        [HttpGet] 
        public async Task<IActionResult> Registro(string returnurl = null) 
        {
            ViewData["ReturnUrl"]= returnurl;
            RegistroVM registroVM = new RegistroVM();
            return View(registroVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registro(RegistroVM registroVM, string returnurl = null)
        {
            ViewData["ReturnUrl"] = returnurl;
            returnurl = returnurl ?? Url.Content("~/");
            if (ModelState.IsValid)
            {
                var usuario = new AppUsuario
                {
                    UserName = registroVM.Email,
                    Email = registroVM.Email,
                    Nombre = registroVM.Nombre,
                    CodigoPais = registroVM.CodigoPais,
                    Telefono = registroVM.Telefono,
                    Pais = registroVM.Pais,
                    Ciudad = registroVM.Ciudad,
                    Direccion = registroVM.Direccion,
                    FechaNacimiento = registroVM.FechaNacimiento,
                    Estado = registroVM.Estado
                };
                var resultado = await _userManager.CreateAsync(usuario, registroVM.Password);
                
                if (resultado.Succeeded)
                {
                    //Implementación de confirmación de email
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(usuario);
                    var urlRetorno = Url.Action("ConfirmarEmail", "Cuentas", new { userId = usuario.Id, code = code }, 
                        protocol: HttpContext.Request.Scheme);

                    var dir = " <a href=\"" + urlRetorno + "\">Click Aquí</a>";
                    await _emailSender.SendEmailAsync(registroVM.Email, "Confirmación de Cuenta - Proy. Identity",
                        "Confirme su Cuenta dando click aquí: " + dir);

                    await _signInManager.SignInAsync(usuario, isPersistent: false);
                    return LocalRedirect(returnurl);
                }
                ValidarErrores(resultado);
            }
            return View(registroVM);          
        }

        private void ValidarErrores(IdentityResult identityResult)
        {
            foreach (var item in identityResult.Errors)
            {
                ModelState.AddModelError(string.Empty, item.Description);
            }
        }
        #endregion

        #region ACCESO
        //Método Mostrar formulario de Acceso
        [HttpGet]
        public IActionResult Acceso(string returnurl = null)
        {
            ViewData["ReturnUrl"] = returnurl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Acceso(AccesoVM accesoVM, string returnurl = null)
        {
            ViewData["ReturnUrl"] = returnurl;
            returnurl = returnurl ?? Url.Content("~/");
            if (ModelState.IsValid)
            {
                var resultado = await _signInManager.PasswordSignInAsync(accesoVM.Email,
                                                                         accesoVM.Password,
                                                                         accesoVM.RememberMe,
                                                                         lockoutOnFailure: true);
                if (resultado.Succeeded)
                {
                    //return RedirectToAction("Index", "Home");
                    return LocalRedirect(returnurl);
                }
                if (resultado.IsLockedOut)
                {
                    return View("Bloqueado");
                };

                ModelState.AddModelError(string.Empty, "Acceso Inválido");
                return View(accesoVM);
            }
            return View(accesoVM);
        }
        #endregion

        #region RECUPERO CONTRASEÑA
        //Método para olvido de Contraseña
        [HttpGet]
        [AllowAnonymous]
        public IActionResult OlvidoPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> OlvidoPassword(OlvidoPasswordVM olvidoPasswordVM)
        {
            if (ModelState.IsValid)
            {
                var usuario = await _userManager.FindByEmailAsync(olvidoPasswordVM.Email);
                if (usuario == null)
                {
                    return RedirectToAction("ConfirmacionOlvidoPassword");
                }
                var codigo = await _userManager.GeneratePasswordResetTokenAsync(usuario);
                var urlRetorno = Url.Action("ResetPassword", "Cuentas", new { userId = usuario.Id, code = codigo }, protocol: HttpContext.Request.Scheme);

                var dir = " <a href=\"" + urlRetorno + "\">Click Aquí</a>";
                await _emailSender.SendEmailAsync(olvidoPasswordVM.Email, "Recuperar contraseña - Proyecto App ",
                    "Por favor recupere su contraseña dando click aquí: " + dir);

                return RedirectToAction("ConfirmacionOlvidoPassword");
            }
            return View(olvidoPasswordVM);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ConfirmacionOlvidoPassword()
        {
            return View();
        }

        //Funcionalidad para Recuperar Contraseña
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string code = null)
        {
            return code == null ? View("Error") : View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(RecuperaPasswordVM recuperaPasswordVM)
        {
            if (ModelState.IsValid)
            {
                var usuario = await _userManager.FindByEmailAsync(recuperaPasswordVM.Email);
                if (usuario == null)
                {
                    return RedirectToAction("ConfirmacionRecuperaPassword");
                }

                var resultado = await _userManager.ResetPasswordAsync(usuario, recuperaPasswordVM.Code, recuperaPasswordVM.Password);
                if (resultado.Succeeded)
                {
                    return RedirectToAction("ConfirmacionRecuperaPassword");
                }
                ValidarErrores(resultado);
                return RedirectToAction("ResetPassword");
            }
            return View(recuperaPasswordVM);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ConfirmacionRecuperaPassword()
        {
            return View();
        }
        
        #endregion

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmarEmail(string userId, string code)
        {
            if (userId == null || code == null) return View("Error");
            var usuario = await _userManager.FindByIdAsync(userId);
            if (usuario == null) return View("Error");
            var resultado = await _userManager.ConfirmEmailAsync(usuario, code);
            return View(resultado.Succeeded ? "ConfirmarEmail" : "Error");
        }

        //Salir o cerrar sesión de la aplicación (logout)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SalirAplicacion()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        #region login con Facebook
        //Configuracion de acceso externo: facebook, google, twitter, etc
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public  IActionResult AccesoExterno(string proveedor, string returnurl = null)
        {
            var urlRedireccion = Url.Action("AccesoExternoCallback", "Cuentas", new { Returnurl = returnurl });
            var propiedades = _signInManager.ConfigureExternalAuthenticationProperties(proveedor, urlRedireccion);
            return Challenge(propiedades, proveedor);
        }

        //Configuracion de acceso externo: facebook, google, twitter, etc
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> AccesoExternoCallback(string returnurl = null, string error = null)
        {
            returnurl = returnurl ?? Url.Action("~/");
            if (error != null)
            {
                ModelState.AddModelError(string.Empty, $"Error en el acceso Externo {error}");
                return View(nameof(Acceso));
            }
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null) return View(nameof(Acceso));

            //Acceder con el usuario en el proveedor externo
            var resultado = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);

            if (resultado.Succeeded)
            {
                await _signInManager.UpdateExternalAuthenticationTokensAsync(info);
                return LocalRedirect(returnurl);
            }
            else
            {
                //si el usuario no tiene cuenta le pregunta si quiere crear una
                ViewData["ReturnUrl"] = returnurl;
                ViewData["NombreAMostrarProveedor"] = info.ProviderDisplayName;
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                var nombre = info.Principal.FindFirstValue(ClaimTypes.Name);
                return View("ConfirmacionAccesoExterno", new ConfirmacionAccesoExternoVM { Email = email, Name = nombre});
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]     
        public async Task<IActionResult> ConfirmacionAccesoExterno(ConfirmacionAccesoExternoVM caeVM, string returnurl = null)
        {
            returnurl = returnurl ?? Url.Content("~/");
            if (ModelState.IsValid)
            {
                //Obtener la información del usuario del proveedor externo
                var info = await _signInManager.GetExternalLoginInfoAsync();
                if (info == null) return View("Error");

                var usuario = new AppUsuario { 
                    UserName = caeVM.Email, 
                    Email = caeVM.Email, 
                    Nombre = caeVM.Name 
                };
                var resultado = await _userManager.CreateAsync(usuario);
                if (resultado.Succeeded)
                {
                    resultado = await _userManager.AddLoginAsync(usuario, info);
                    if (resultado.Succeeded) await _signInManager.SignInAsync(usuario, isPersistent:false);
                    await _signInManager.UpdateExternalAuthenticationTokensAsync(info);
                    return LocalRedirect(returnurl);
                }
                ValidarErrores(resultado);
                ViewData["Returnurl"] = returnurl;
                return View(caeVM);
            }
            return View(caeVM);
        }
        #endregion

        #region Logeo con GOOGLE


        #endregion
    }
}
