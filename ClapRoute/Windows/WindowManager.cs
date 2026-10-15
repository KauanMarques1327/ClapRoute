using System;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace ClapRoute.Windows;

public class WindowManager //gerencia janela do windows
{
    private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam); //tipo que representa uma referência a um método

    [DllImport("user32.dll")] //permite que sejam chamadas funções nativas da API do Windows

    private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam); //EnumWindows percorre as janelas de
                                                                                       //nível superior existentes no Windows  

    [DllImport("user32.dll")]
    private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount); //GetWindowText obtém o título
                                                                                                 //de uma janela

    [DllImport("user32.dll")]
    private static extern bool IsWindowVisible(IntPtr hWnd); //verifica se a janela está visível

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam); //envia uma mensagem para a
                                                                                                 //fila de msg associado a janela

    [DllImport("user32.dll")]
    private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);
    private const uint WM_CLOSE = 0x0010;

    public IntPtr EncontrarJanelaChatGPT()
    {
        IntPtr janelaEncontrada = IntPtr.Zero;

        EnumWindows((hWnd, lParam) =>
        {
            if (!IsWindowVisible(hWnd))
            {
                return true;
            }

            StringBuilder titulo = new StringBuilder(256);
            GetWindowText(hWnd, titulo, titulo.Capacity);

            if (titulo.ToString() != "ChatGPT")
            {
                return true;
            }

            GetWindowThreadProcessId(hWnd, out uint processId);

            try
            {
                using Process processo = Process.GetProcessById((int)processId);

                if (processo.ProcessName.Equals("chrome", StringComparison.OrdinalIgnoreCase))
                {
                    janelaEncontrada = hWnd;
                    return false;
                }
            }
            catch (ArgumentException)
            {
                //o processo pode ter sido encerrado
            }

            catch (InvalidOperationException)
            {
                //o processo pode não estar mais disponível
            }
            return true;
        }, IntPtr.Zero);

        return janelaEncontrada;
    }

    public void ListarJanelas()
    {
        EnumWindows((hWnd, lParam) =>
        {
            if (IsWindowVisible(hWnd))
            {
                StringBuilder titulo = new StringBuilder(256); //classe utilizada para construir e modificar textos, nesse caso
                                                               //funciona como um buffer para receber o título da janela

                GetWindowText(hWnd, titulo, titulo.Capacity);

                if (titulo.Length > 0)
                {
                    Console.WriteLine($"HWND: {hWnd} | Título: {titulo}");
                }
            }
            return true;
        }, IntPtr.Zero);
    }

    public void FecharChatGPT()
    {
        IntPtr janela = EncontrarJanelaChatGPT();

        if (janela == IntPtr.Zero)
        {
            Console.WriteLine("Janela do ChatGPT não encontrada.");
            return;
        }
        bool enviado = PostMessage(janela, WM_CLOSE, IntPtr.Zero, IntPtr.Zero); //enviamos a msg de fechamento

        if (enviado)
        {
            Console.WriteLine("Fechamento do ChatGPT solicitado");
        }
        else
        {
            Console.WriteLine("Não foi possível enviar o fechamento");
        }

    }
}