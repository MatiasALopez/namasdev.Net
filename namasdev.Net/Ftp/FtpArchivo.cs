using System;

using namasdev.Core.Validation;

namespace namasdev.Net.Ftp
{
    public class FtpArchivo : IFtpEntrada
    {
        private string _nombre;

        public FtpArchivo(Uri uri)
        {
            Validador.ValidarArgumentRequeridoYThrow(uri, nameof(uri));

            this.Uri = uri;
        }

        public string Nombre 
        {
            get { return _nombre ?? (_nombre = FtpEntradaUtilidades.ObtenerNombreArchivoDesdeUri(this.Uri)); } 
        }

        public Uri Uri { get; private set; }

        public override string ToString()
        {
            return this.Nombre;
        }
    }
}
