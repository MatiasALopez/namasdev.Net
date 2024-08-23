using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

using namasdev.Core.Validation;

namespace namasdev.Net.Ftp
{
    public class FtpCliente
    {
        private const int CARGA_ARCHIVO_TAMAÑO_BUFFER = 2048;
        private const int DESCARGA_ARCHIVO_TAMAÑO_BUFFER = 2048;

        public FtpCliente(Uri uri)
        {
            Validador.ValidarArgumentRequeridoYThrow(uri, nameof(uri));

            this.Uri = uri;
        }

        public Uri Uri { get; private set; }
        public ICredentials Credenciales { get; set; }

        public IFtpEntrada[] ObtenerEntradas(string nombreDirectorio)
        {
            var nombresEntradas = ObtenerNombresEntradas(nombreDirectorio);

            var entradas = new List<IFtpEntrada>();
            string nombreEntradaSinDirectorio  = null;
            Uri entradaUri = null;
            foreach (var nombreEntrada in nombresEntradas)
            {
                nombreEntradaSinDirectorio = FtpEntradaUtilidades.ObtenerNombreEntradaSinDirectorio(nombreEntrada);
                entradaUri = FtpEntradaUtilidades.CrearEntradaUri(this.Uri, nombreDirectorio, nombreEntradaSinDirectorio);

                if (FtpEntradaUtilidades.EsArchivo(nombreEntrada))
                {
                    entradas.Add(new FtpArchivo(entradaUri));
                }
                else
                {
                    entradas.Add(new FtpDirectorio(entradaUri, ObtenerEntradas(FtpEntradaUtilidades.CombinarPartesUri(nombreDirectorio, nombreEntradaSinDirectorio))));
                }
            }

            return entradas.ToArray();
        }

        private string[] ObtenerNombresEntradas(string directorio = null)
        {
            var request = CrearRequestConCredenciales(new Uri(this.Uri, directorio), WebRequestMethods.Ftp.ListDirectory);

            var res = new List<string>();

            using (var response = (FtpWebResponse)request.GetResponse())
            using (var responseStream = response.GetResponseStream())
            using (var reader = new StreamReader(responseStream))
            {
                while (!reader.EndOfStream)
                {
                    res.Add(reader.ReadLine());
                }

                return res.ToArray();
            }
        }

        public void DescargarArchivo(Uri uri, string pathArchivoDestino)
        {
            var request = CrearRequestConCredenciales(uri, WebRequestMethods.Ftp.DownloadFile);

            request.UseBinary = true;
            request.Proxy = null;

            using (var response = (FtpWebResponse)request.GetResponse())
            {
                ValidarResponseConStatusCodeEsperado(response);

                using (var responseStream = response.GetResponseStream())
                using (var fileStream = new FileStream(pathArchivoDestino, FileMode.Create))
                {
                    byte[] buffer = new byte[DESCARGA_ARCHIVO_TAMAÑO_BUFFER];
                    while(true)
                    {
                        int readCount = responseStream.Read(buffer, 0, buffer.Length);
                        if (readCount == 0)
                        {
                            break;
                        }

                        fileStream.Write(buffer, 0, readCount);
                    }
                }
            }
        }

        public void EliminarArchivo(Uri uri)
        {
            var request = CrearRequestConCredenciales(uri, WebRequestMethods.Ftp.DeleteFile);

            using (var response = (FtpWebResponse)request.GetResponse())
            {
                ValidarResponseConStatusCodeEsperado(response);
            }
        }

        public void EliminarDirectorio(Uri uri)
        {
            var request = CrearRequestConCredenciales(uri, WebRequestMethods.Ftp.RemoveDirectory);
            
            using (var response = (FtpWebResponse)request.GetResponse())
            {
                ValidarResponseConStatusCodeEsperado(response);
            }
        }

        public void CrearDirectorioSiNoExiste(string[] directories)
        {
            var uri = FtpEntradaUtilidades.CrearDirectorioUri(
                uriBase: this.Uri,
                direccionCompletaDirectorio: FtpEntradaUtilidades.ObtenerDireccionCompletaDirectorio(directories)
            );

            try
            {
                var request = CrearRequestConCredenciales(uri, WebRequestMethods.Ftp.MakeDirectory);

                using (var response = (FtpWebResponse)request.GetResponse())
                {
                    ValidarResponseConStatusCodeEsperado(response, FtpStatusCode.PathnameCreated);
                }
            }
            catch (WebException ex)
            {
                // This happens when the folder is already created...
                if (ex.Status != WebExceptionStatus.ProtocolError)
                {
                    throw;
                }
            }
        }

        public void CargarArchivo(string pathArchivo,
            string nombreArchivo = null, string[] directories = null)
        {
            var uri = FtpEntradaUtilidades.CrearEntradaUri(
                uriBase: this.Uri,
                nombreDirectorio: FtpEntradaUtilidades.CombinarPartesUri(directories),
                nombreEntrada: nombreArchivo ?? Path.GetFileName(pathArchivo)
            );

            var request = CrearRequestConCredenciales(uri, WebRequestMethods.Ftp.UploadFile);

            request.UseBinary = true;
            request.Proxy = null;

            using (var requestStream = request.GetRequestStream())
            using (var fileStream = System.IO.File.OpenRead(pathArchivo))
            {
                byte[] buffer = new byte[CARGA_ARCHIVO_TAMAÑO_BUFFER];
                while (true)
                {
                    int readCount = fileStream.Read(buffer, 0, buffer.Length);
                    if (readCount == 0)
                    {
                        break;
                    }

                    requestStream.Write(buffer, 0, readCount);
                }
            }

            using (var response = (FtpWebResponse)request.GetResponse())
            {
                ValidarResponseConStatusCodeEsperado(response, FtpStatusCode.ClosingData);
            }
        }

        private FtpWebRequest CrearRequestConCredenciales(Uri uri, string metodo)
        {
            var request = (FtpWebRequest)FtpWebRequest.CreateDefault(uri);
            request.Method = metodo;

            if (this.Credenciales != null)
            {
                request.Credentials = this.Credenciales;
            }

            return request;
        }

        private void ValidarResponseConStatusCodeEsperado(FtpWebResponse response,
            FtpStatusCode statusCodeEsperado = FtpStatusCode.FileActionOK)
        {
            if (response.StatusCode != statusCodeEsperado)
            {
                throw new FtpException(response.StatusCode, response.StatusDescription);
            }
        }

        public override string ToString()
        {
            return this.Uri.AbsoluteUri;
        }
    }
}
