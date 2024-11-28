using System;
using System.IO;
using System.Text;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;

class Program
{
    // static void Main()
    // {
        // string arquivoPdf = @"C:\Tareffa";
        
        // // Abrindo o PDF
        // PdfDocument pdfDocument = new PdfDocument(new PdfReader(arquivoPdf));
        // StringBuilder texto = new StringBuilder();

        // // Loop para ler página por página
        // for (int i = 1; i <= pdfDocument.GetNumberOfPages(); i++)
        // {
        //     var page = pdfDocument.GetPage(i);
        //     // Usa PdfTextExtractor para extrair texto da página
        //     string textoDaPagina = PdfTextExtractor.GetTextFromPage(page);
        //     texto.Append(textoDaPagina);
        // }

        // // Exibindo o texto extraído
        // Console.WriteLine(texto.ToString());
        
        // pdfDocument.Close();
    // }

    static void Main()
    {
        // Caminho da pasta de origem
        string folderPath = @"C:\Tareffa";
        string sentFolder = Path.Combine(folderPath, "ENVIADOS");

        try
        {
            // Verifica se a pasta "ENVIADOS" existe, caso contrário, cria
            if (!Directory.Exists(sentFolder))
            {
                Directory.CreateDirectory(sentFolder);
            }

            // Obtém todos os arquivos da pasta
            string[] files = Directory.GetFiles(folderPath);

            Console.WriteLine($"Arquivos encontrados na pasta '{folderPath}':");

            foreach (string file in files)
            {
                try
                {
                    Console.WriteLine($"Lendo arquivo: {Path.GetFileName(file)}");

                    string content = string.Empty;

                    // Lê o conteúdo de arquivos de texto (.txt)
                    if (Path.GetExtension(file).Equals(".txt", StringComparison.OrdinalIgnoreCase))
                    {
                        content = File.ReadAllText(file, Encoding.UTF8);
                    }

                    // Lê o conteúdo de arquivos PDF (.pdf)
                    else if (Path.GetExtension(file).Equals(".pdf", StringComparison.OrdinalIgnoreCase))
                    {
                        using (PdfReader pdfReader = new PdfReader(file))
                        using (PdfDocument pdfDocument = new PdfDocument(pdfReader))
                        {
                            var strategy = new SimpleTextExtractionStrategy();
                            for (int i = 1; i <= pdfDocument.GetNumberOfPages(); i++)
                            {
                                var page = pdfDocument.GetPage(i);
                                content += PdfTextExtractor.GetTextFromPage(page, strategy);
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Formato de arquivo não suportado.");
                        continue;
                    }

                    // Exibe o conteúdo do arquivo no console
                    Console.WriteLine("Conteúdo do arquivo:");
                    Console.WriteLine(content);
                    Console.WriteLine(new string('-', 50)); // Separador visual

                    // Move o arquivo para a pasta "ENVIADOS"
                    string destinationPath = Path.Combine(sentFolder, Path.GetFileName(file));
                    File.Move(file, destinationPath);
                    Console.WriteLine($"Arquivo movido para: {destinationPath}");
                }
                catch (Exception ex)
                {
                    // Captura erros ao tentar ler ou mover o arquivo
                    Console.WriteLine($"Erro ao processar o arquivo '{Path.GetFileName(file)}': {ex.Message}");
                }
            }
        }
        catch (DirectoryNotFoundException)
        {
            Console.WriteLine($"A pasta '{folderPath}' não existe.");
        }
        catch (UnauthorizedAccessException)
        {
            Console.WriteLine("Você não tem permissão para acessar esta pasta ou arquivos.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ocorreu um erro inesperado: {ex.Message}");
        }
    }
}
