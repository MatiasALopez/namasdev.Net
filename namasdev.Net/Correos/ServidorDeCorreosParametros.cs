using System.Collections.Generic;
using System.Net;

namespace namasdev.Net.Correos
{
    public class ServidorDeCorreosParametros
    {
        public ServidorDeCorreosParametros()
        {
        }

        public ServidorDeCorreosParametros(string host,
            int? puerto = null, NetworkCredential credenciales = null, bool? habilitarSsl = null,
            string remitente = null, string copiaOculta = null,
            IEnumerable<KeyValuePair<string, string>> headers = null)
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
        public IEnumerable<KeyValuePair<string, string>> Headers { get; set; }
        public string PickupDirectory { get; set; }
    }
}
