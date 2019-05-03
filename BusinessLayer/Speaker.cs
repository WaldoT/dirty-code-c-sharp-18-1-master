using System;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLayer
{
    /// <summary>
    /// Represents a single speaker
    /// </summary>
    public class Speaker
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int? Exp { get; set; }
        public bool HasBlog { get; set; }
        public string BlogURL { get; set; }
        public WebBrowser Browser { get; set; }
        public List<string> Certifications { get; set; }
        public string Employer { get; set; }
        public int RegistrationFee { get; set; }
        public List<BusinessLayer.Session> Sessions { get; set; }

        //Realizando Cambios según la Sesion 03 - Codigo Limpio
        const int intCero = 0;
        const int intUno = 1;
        const int intTres = 3;
        const int intNueve = 9;
        const int intDiez = 10;
        const int intCincuenta = 50;
        const int intCien = 100;
        const int intDocientosCincuenta = 250;
        const int intQuinientos = 500;

        /// <summary>
        /// Register a speaker
        /// </summary>
        /// <returns>speakerID</returns>
        public int? RegisterSpeaker(IRepository repository)
        {
            //lets init some vars
            int? speakerId = intCero;

            //We weren't filtering out the prodigy domain so I added it.
            var domains = new List<string>() { "aol.com", "hotmail.com", "prodigy.com", "CompuServe.com" };

            //Now, save the speaker and sessions to the db.
            try
            {
                if (ValidarDatos(domains))
                {
                    speakerId = repository.SaveSpeaker(this);
                }

            }
            catch (Exception e)
            {
                //in case the db call fails 
            }

            //if we got this far, the speaker is registered.
            return speakerId;
        }

        public bool ValidarDatos(List<string> domains)
        {
            //lets init some vars
            bool IsGood = false;
            bool CondicionAppr = false;
            var ListOt = new List<string>() { "Cobol", "Punch Cards", "Commodore", "VBScript" };
            bool Validacion = false;

            if (ValidarCampos())
            {

                //put list of employers in array
                var employers = new List<string>() { "Microsoft", "Google", "Fog Creek Software", "37Signals" };

                //We're now requiring 3 certifications so I changed the hard coded number. Boy, programming is hard.
                IsGood = ((Exp > intDiez || HasBlog || Certifications.Count() > intTres || employers.Contains(Employer)));

                if (!IsGood)
                {
                    //need to get just the domain from the email
                    string emailDomain = Email.Split('@').Last();

                    IsGood = !domains.Contains(emailDomain) && (!(Browser.Name == WebBrowser.BrowserName.InternetExplorer && Browser.MajorVersion < intNueve));

                }

                if (IsGood)
                {
                    CondicionAppr = ValidaCondicionAppr(ListOt);

                    var validarRagoUno = new List<int>() { 2, 3 };
                    var validarRagoDos = new List<int>() { 4, 5 };
                    var validarRagoTres = new List<int>() { 6, 7, 8, 9 };

                    if (CondicionAppr)
                    {
                        //if we got this far, the speaker is approved
                        //let's go ahead and register him/her now.
                        //First, let's calculate the registration fee. 
                        //More experienced speakers pay a lower fee.
                        RegistrationFee = intCero;

                        if (Exp <= intUno)
                        {
                            RegistrationFee = intQuinientos;
                        }
                        if (validarRagoUno.Contains(Convert.ToInt32(Exp)))
                        {
                            RegistrationFee = intDocientosCincuenta;
                        }
                        if (validarRagoDos.Contains(Convert.ToInt32(Exp)))
                        {
                            RegistrationFee = intCien;
                        }
                        if (validarRagoTres.Contains(Convert.ToInt32(Exp)))
                        {
                            RegistrationFee = intCincuenta;
                        }

                        Validacion = CondicionAppr;
                    }
                    else
                    {
                        throw new NoSessionsApprovedException("No sessions approved.");
                    }
                }
                else
                {
                    throw new SpeakerDoesntMeetRequirementsException("Speaker doesn't meet our abitrary and capricious standards.");
                }

            }

            return Validacion;
        }

        public bool ValidaCondicionAppr(List<string> ListOt)
        {
            bool CondicionAppr = false;

            //We weren't requiring at least one session
            if (Sessions.Count() != intCero)
            {
                foreach (var session in Sessions)
                {
                    foreach (var tech in ListOt)
                    {
                        if (session.Title.Contains(tech) || session.Description.Contains(tech))
                        {
                            session.Approved = false;
                            break;
                        }
                        else
                        {
                            session.Approved = true;
                            CondicionAppr = true;
                        }
                    }
                }
            }
            else
            {
                throw new ArgumentException("Can't register speaker with no sessions to present.");
            }

            return CondicionAppr;
        }

        public bool ValidarCampos()
        {
            bool validarCampos = false;

            if (string.IsNullOrWhiteSpace(FirstName))
            {
                throw new ArgumentNullException("First Name is required");
            }
            else
            {
                validarCampos = true;
            }

            if (string.IsNullOrWhiteSpace(LastName))
            {
                throw new ArgumentNullException("Last name is required.");
            }
            else
            {
                validarCampos = true;
            }

            if (string.IsNullOrWhiteSpace(Email))
            {
                throw new ArgumentNullException("Email is required.");
            }
            else
            {
                validarCampos = true;
            }

            return validarCampos;
        }

        #region Custom Exceptions
        public class SpeakerDoesntMeetRequirementsException : Exception
        {
            public SpeakerDoesntMeetRequirementsException(string message)
                : base(message)
            {
            }

            public SpeakerDoesntMeetRequirementsException(string format, params object[] args)
                : base(string.Format(format, args)) { }
        }

        public class NoSessionsApprovedException : Exception
        {
            public NoSessionsApprovedException(string message)
                : base(message)
            {
            }
        }
        #endregion
    }
}