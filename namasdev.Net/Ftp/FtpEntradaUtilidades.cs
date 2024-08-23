using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace namasdev.Net.Ftp
{
    public static class FtpEntradaUtilidades
    {
        private const string SEPARADOR_URI = "/";

        public static bool EsArchivo(string nombreEntrada)
        {
            return Path.HasExtension(nombreEntrada);
        }

        public static string ObtenerNombreArchivoDesdeUri(Uri uri)
        {
            return ObtenerNombreEntradaDesdeUri(uri);
        }

        public static string ObtenerNombreDirectorioDesdeUri(Uri uri)
        {
            return ObtenerNombreEntradaDesdeUri(uri);
        }

        private static string ObtenerNombreEntradaDesdeUri(Uri uri)
        {
            return ObtenerNombreEntradaSinDirectorio(uri.LocalPath);
        }

        public static string ObtenerNombreEntradaSinDirectorio(string nombreEntrada)
        {
            return Path.GetFileName(nombreEntrada);
        }

        public static string ObtenerDireccionCompletaDirectorio(string[] nombresDirectorios)
        {
            if (nombresDirectorios == null)
            {
                return String.Empty;
            }

            return CombinarPartesUri(nombresDirectorios
                .Where(it => !String.IsNullOrWhiteSpace(it))
                .Select(it => it.TrimEnd(SEPARADOR_URI.ToCharArray()))
                .ToArray());
        }

        public static Uri CrearEntradaUri(Uri uriBase, string nombreDirectorio, string nombreEntrada)
        {
            return new Uri(uriBase, CombinarPartesUri(nombreDirectorio, nombreEntrada));
        }

        public static Uri CrearDirectorioUri(Uri uriBase, string direccionCompletaDirectorio)
        {
            if (String.IsNullOrWhiteSpace(direccionCompletaDirectorio))
            {
                return uriBase;
            }

            if (!direccionCompletaDirectorio.EndsWith(SEPARADOR_URI))
            {
                direccionCompletaDirectorio += SEPARADOR_URI;
            }

            return new Uri(uriBase, direccionCompletaDirectorio);
        }

        public static string CombinarPartesUri(params string[] partesUri)
        {
            return String.Join(SEPARADOR_URI, partesUri);
        }
    }
}
