﻿using System;
using System.Net.Mail;

using namasdev.Core.Validation;

namespace namasdev.Net.Correos
{
    public class ServidorDeCorreos : IServidorDeCorreos, IDisposable
    {
        private SmtpClient _smtpClient;

        public ServidorDeCorreos(ServidorDeCorreosParametros parametros)
        {
            Parametros = parametros;

            InicializarSmtp();
        }

        #region Propiedades

        public ServidorDeCorreosParametros Parametros { get; private set; }
        
        #endregion

        #region Metodos

        private void InicializarSmtp()
        {
            _smtpClient = new SmtpClient(Parametros.Host);

            if (Parametros.Puerto.HasValue)
            {
                _smtpClient.Port = Parametros.Puerto.Value;
            }

            if (Parametros.Credenciales != null)
            {
                _smtpClient.Credentials = Parametros.Credenciales;
            }

            if (Parametros.HabilitarSsl.HasValue)
            {
                _smtpClient.EnableSsl = Parametros.HabilitarSsl.Value;
            }
        }

        public void EnviarCorreo(MailMessage correo)
        {
            Validador.ValidarArgumentRequeridoYThrow(correo, nameof(correo));

            EstablecerRemitent(correo);
            EstablecerHeaders(correo);
            EstablecerCopiaOculta(correo);

            _smtpClient.Send(correo);
        }

        private void EstablecerRemitent(MailMessage correo)
        {
            if (correo.Sender == null
                && !string.IsNullOrWhiteSpace(Parametros.Remitente))
            {
                correo.Sender = new MailAddress(Parametros.Remitente);
            }
        }

        private void EstablecerHeaders(MailMessage correo)
        {
            if (Parametros.Headers != null)
            {
                correo.Headers.Add(Parametros.Headers);
            }
        }

        private void EstablecerCopiaOculta(MailMessage correo)
        {
            if (!string.IsNullOrWhiteSpace(Parametros.CopiaOculta))
            {
                correo.Bcc.Add(Parametros.CopiaOculta);
            }
        }

        #endregion

        public void Dispose()
        {
            if (_smtpClient != null)
            {
                _smtpClient.Dispose();
            }
        }
    }
}
