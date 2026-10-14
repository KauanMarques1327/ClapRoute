using System.Diagnostics; //importa classes relacionadas a processos, diagnósticos e execução de programas

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
        Process.Start(new ProcessStartInfo { FileName = "chrome.exe", Arguments = "https://chatgpt.com", UseShellExecute = true });
        //Windows abra o chrome e passe o endereço do ChatGPT para ele
    }
}