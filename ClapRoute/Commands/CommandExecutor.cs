using System.Diagnostics;
using ClapRoute.Windows;

namespace ClapRoute.Commands;

public class CommandExecutor  // Armazena as janelas abertas pelo ClapRoute.
{
    private readonly Dictionary<string, (IntPtr Janela, int Processo)> janelasAbertas = new();
    //readonly: impede que o campo receba outro dicionário depois da inicialização
    //Dictionary: uma coleção que armazena pares de chave e valor
    //(IntPtr Janela, int Processo): uma tupla que armazena HWND e PID
    //IntPtr: inteiro de ponteiro, usado para guardar HWND(Handle to a Window)
    //IntPtr.Zero representa um identificador ou ponteiro de valor zero.
    private readonly WindowManager gerenciador = new();
    private readonly object trava = new();

    // Abre o VS Code.
    private void AbrirVSCode()
    {
        Process.Start(new ProcessStartInfo { FileName = "code", UseShellExecute = true });
    }

    private void FecharVSCode()
    {
        Process[] processos = Process.GetProcessesByName("Code");

        foreach (Process processo in processos)
        {
            try
            {
                if (processo.MainWindowHandle != IntPtr.Zero) //é uma propriedade que retorna o identificador da janela principal 
                {
                    bool fechamentoSolicitado = processo.CloseMainWindow();

                    if (fechamentoSolicitado)
                    {
                        Console.WriteLine("Fechamento do VS Code solicitado.");
                    }
                }
            }
            catch (InvalidOperationException)
            {
                // O processo pode ter sido encerrado durante a verificação.
            }
            finally
            {
                processo.Dispose(); //Libera recursos não gerenciados de forma imediata e determinística sem esperar o GC
            }
        }
    }

    private void AlternarVSCode() //Alterna entre abrir e fechar o VS Code.
    {
        bool vscodeAberto = false;

        foreach (Process processo in Process.GetProcessesByName("Code"))
        {
            try
            {
                if (processo.MainWindowHandle != IntPtr.Zero) //
                {
                    vscodeAberto = true;
                }
            }
            catch (InvalidOperationException)
            {
                // O processo pode ter sido encerrado durante a verificação.
            }
            finally
            {
                processo.Dispose();
            }
        }

        if (vscodeAberto)
        {
            Console.WriteLine("VS Code aberto. Solicitando fechamento...");
            FecharVSCode();
        }
        else
        {
            Console.WriteLine("VS Code fechado. Abrindo...");
            AbrirVSCode();
        }
    }

    // Executa o comando associado ao aplicativo escolhido.
    public void ExecutarAplicativo(string aplicativo)
    {
        switch (aplicativo)
        {
            case "VS Code":
                AlternarVSCode();
                break;

            case "ChatGPT":
                AlternarSite("ChatGPT", "https://chatgpt.com");
                break;

            case "YouTube":
                AlternarSite("YouTube", "https://www.youtube.com");
                break;

            case "WhatsApp Web":
                AlternarSite("WhatsApp Web", "https://web.whatsapp.com");
                break;

            case "Instagram":
                AlternarSite("Instagram", "https://www.instagram.com");
                break;

            case "Notion":
                AlternarSite("Notion", "https://www.notion.so");
                break;

            case "Gemini":
                AlternarSite("Gemini", "https://gemini.google.com");
                break;

            case "Claude":
                AlternarSite("Claude", "https://claude.ai");
                break;

            default:
                Console.WriteLine($"O aplicativo {aplicativo} ainda não possui comando configurado.");
                break;
        }
    }

    private void AbrirSite(string url) // Abre um site em uma janela independente do Chrome.
    {
        Process.Start(new ProcessStartInfo { FileName = "chrome.exe", Arguments = $"--app=\"{url}\"", UseShellExecute = true });
    }

    private void AlternarSite(string aplicativo, string url) // Alterna entre abrir e fechar uma janela de site
    {
        lock (trava) //Mecanismos de sincronização entre threads
        {
            if (janelasAbertas.TryGetValue(aplicativo, out var registro))//Verifica se o aplicativo possui uma janela registrada.
            {
                if (gerenciador.JanelaChromeValida(registro.Janela, registro.Processo))
                {
                    Console.WriteLine($"{aplicativo} aberto. Solicitando fechamento...");

                    bool fechamentoSolicitado = gerenciador.FecharJanela(registro.Janela);

                    if (fechamentoSolicitado)
                    {
                        janelasAbertas.Remove(aplicativo);
                    }
                    else
                    {
                        Console.WriteLine("Não foi possível solicitar o fechamento.");
                    }

                    return;
                }

                janelasAbertas.Remove(aplicativo); // Descarta registros de janelas que não existem mais.
            }

            Console.WriteLine($"{aplicativo} fechado. Abrindo...");

            HashSet<IntPtr> janelasAntes = gerenciador.ObterJanelasChrome(); //Registra janelas existentes antes de abrir o site.
            //HashSet não permite elementos repetidos

            AbrirSite(url);

            IntPtr novaJanela = IntPtr.Zero; //Inicio a variável novaJanela com valor 0

            for (int tentativa = 0; tentativa < 20; tentativa++)
            {
                Thread.Sleep(200);

                HashSet<IntPtr> janelasDepois = gerenciador.ObterJanelasChrome();

                janelasDepois.ExceptWith(janelasAntes); //Mantém apenas as janelas que não existiam antes.

                if (janelasDepois.Count == 1)
                {
                    novaJanela = janelasDepois.First(); //retorna o primeiro elemento da coleção
                    break;
                }

                if (janelasDepois.Count > 1)
                {
                    Console.WriteLine("Mais de uma janela apareceu. Registro cancelado.");
                    return;
                }
            }

            if (novaJanela == IntPtr.Zero)
            {
                Console.WriteLine("Não foi possível identificar a janela aberta.");
                return;
            }

            int processo = gerenciador.ObterProcessoDaJanela(novaJanela); //processo armazena o PID (Process Identifier)

            if (processo == 0)
            {
                Console.WriteLine("Não foi possível validar o processo da janela.");
                return;
            }

            janelasAbertas[aplicativo] = (novaJanela, processo);
            //janelasAbertas é uma variável que armazena um dicionário. Cada chave é uma string contendo o nome do aplicativo
            //cada valor dessa variável é uma tupla contendo o HWND e o PID

            Console.WriteLine($"{aplicativo} registrado com sucesso.");

        }
    }
}