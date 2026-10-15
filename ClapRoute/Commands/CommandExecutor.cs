using System.Diagnostics; //importa classes relacionadas a processos, diagnósticos e execução de programas
using ClapRoute.Windows;

namespace ClapRoute.Commands;

public class CommandExecutor
{
    public void AbrirVSCode()
    {
        Process.Start(new ProcessStartInfo { FileName = "code", UseShellExecute = true });
        //Process.Start é um método que solicita ao sistema operacional que inicie um processo
        //PrcecessStartInfo é uma classe que permite configurar como um processo será iniciado
        //Em seguida é indicado o comando utilizado para iniciar o VS Code e solicita que o Windows Shell execute
    }

    public void AbrirChatGPT()
    {
        Process.Start(new ProcessStartInfo { FileName = "chrome.exe", Arguments = "--app=https://chatgpt.com", UseShellExecute = true });
        //Windows abra o chrome e passe o endereço do ChatGPT para ele
        //--app faz o ChatGPT abrir em uma janela independente
    }

    public void FecharVSCode()
    {
        Process[] processos = Process.GetProcessesByName("Code"); //Process é uma classe do .NET que representa os processos do SO
                                                                  //Process[] é um array de objetos dessa classe
                                                                  //GetProcessesByName procura processos cujo nome seja (...)
        foreach (Process processo in processos)
        {
            try
            {
                if (processo.MainWindowHandle != IntPtr.Zero) //Se o processo possui uma janela principal identificável,
                                                              //podemos tentar solicitar seu fechamento.
                {
                    bool fechamentoSolicitado = processo.CloseMainWindow(); //Solicita que a janela seja fechada normalmente.

                    if (fechamentoSolicitado)
                    {
                        Console.WriteLine("Fechamento do VS Code solicitado");
                    }
                }
            }
            catch (InvalidOperationException)
            {
                Console.WriteLine("Um processo do VS Code já foi encerrado");
            }
            finally
            {
                processo.Dispose();
            }
        }
    }

    public void AlternarVSCode()
    {
        Process[] processos = Process.GetProcessesByName("Code");

        bool vscodeAberto = false;

        foreach (Process processo in processos)
        {
            try
            {
                if (processo.MainWindowHandle != IntPtr.Zero) //Quando encontramos um processo, atualizamos a variável
                {
                    vscodeAberto = true;
                }
            }
            catch (InvalidOperationException)
            {
                //O processo pode ter sido encerrado durante a verificação
            }
            finally
            {
                processo.Dispose(); //liberar manualmente recursos não gerenciados
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

    public void AlternarChatGPT()
    {
        WindowManager gerenciador = new WindowManager();

        IntPtr janela = gerenciador.EncontrarJanelaChatGPT();

        if (janela != IntPtr.Zero)
        {
            Console.WriteLine("ChatGPT aberto. Solicitando fechamento...");
            gerenciador.FecharChatGPT();
        }

        else
        {
            Console.WriteLine("ChatGPT fechado. Abrindo...");
            AbrirChatGPT();
        }
    }
    public void ExecutarAplicativo(string aplicativo)
    {
        switch (aplicativo)
        {
            case "VS Code":
                AlternarVSCode();
                break;

            case "ChatGPT":
                AlternarChatGPT();
                break;

            default:
                Console.WriteLine(
                    $"O aplicativo {aplicativo} ainda não possui comando configurado."
                );
                break;
        }
    }
}