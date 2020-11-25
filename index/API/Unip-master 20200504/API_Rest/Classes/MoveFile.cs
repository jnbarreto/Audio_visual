using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using API_Rest.Classes;
using API_Rest.Models;
using System.Security.Permissions;

namespace API_Rest.Classes
{
    public static class MoveFile
    {
        public static string move(string nom_file, Link link)
        {
            DB Conn = new DB();
            string path = @"C:\Users\Matheus Machado\Documents\" + nom_file;
            //string path = @"C:\Users\Administrator\Documents\" + nom_file;

            string path2 = @"D:\projetos\audiovisual\backend\arquivos\xlsx\" + nom_file;
            //string path2 = @"C:\projeto\audiovisual\backend\arquivos\xlsx\" + nom_file;

            if (Directory.Exists(@"D:\projetos\audiovisual\backend\arquivos"))
            {
                link.link = path2;
                try
                {
                    if (!File.Exists(path))
                    {
                        // Esta declaração garante que o arquivo seja criado,
                        // mas a alça não é mantida.
                        using (FileStream fs = File.Create(path)) { }
                    }

                    // Certifique - se de que o destino não exista.
                    if (File.Exists(path2))
                        File.Delete(path2);

                    // Mova o arquivo.
                    File.Move(path, path2);
                    Conn.debug(" " + path + " foi movido para " + path2 + ".", "", true);

                    // Veja se o original existe agora
                    if (File.Exists(path))
                    {
                        Conn.debug("O arquivo original ainda existe, o que é inesperado", "", true);
                    }
                    else
                    {
                        Conn.debug("O arquivo original não existe mais, o que é esperado", "", true);
                    }
                }
                catch (Exception e)
                {
                    Conn.debug("Falha ao mover o Arquivo. ERRO: " + e.ToString(), "", true);
                    throw new Exception("Falha ao mover o Arquivo. ERRO: " + e.ToString());
                }
            }
            else
            {
                string caminho = "D:\\projetos\\audiovisual\\backend\\arquivos";
                caminho = caminho.Replace("\\", "/");
                DirectoryInfo dir = Directory.CreateDirectory(caminho);

                string caminho2 = "D:\\projetos\\audiovisual\\backend\\arquivos\\xlsx";
                caminho2 = caminho2.Replace("\\", "/");

                DirectoryInfo dir2 = Directory.CreateDirectory(caminho2);

                move(nom_file, link);
            }
            
            return path2;
        }

    }
}   