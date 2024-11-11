using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace namasdev.Net.Correos
{
    public class ServidorDeCorreosParametros
    {
        public ServidorDeCorreosParametros(string host,
            int? puerto = null, NetworkCredential credenciales = null, bool? habilitarSsl = null,
            string remitente = null, string copiaOculta = null, 
            NameValueCollection headers = null)
        {
            Host = host;
            Puerto = puerto;
            Credenciales = credenciales;
            HabilitarSsl = habilitarSsl;
            Remitente = remitente;
            CopiaOculta = copiaOculta;
            Headers = headers;
        }

        public string Host { get; set; }
        public int? Puerto { get; set; }
        public NetworkCredential Credenciales { get; set; }
        public bool? HabilitarSsl { get; set; }
        public string Remitente { get; set; }
        public string CopiaOculta { get; set; }
        public NameValueCollection Headers { get; set; }
    }
}
