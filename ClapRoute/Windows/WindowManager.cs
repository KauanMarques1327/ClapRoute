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

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]//trabalharemos com caracteres Unicode, permitindo obter títulos e 
                                                        //nomes que contenham acentos e outros caracteres.
    private static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);
    //IntPtr hWnd: identificador da janela que queremos investigar.
    //StringBuilder lpClassName: espaço onde o Windows escreverá o nome da classe da janela.
    //int nMaxCount: capacidade máxima do buffer, incluindo o terminador nulo.


    [DllImport("user32.dll")]
    private static extern bool IsWindow(IntPtr hWnd); //função que recebe um identificador hwnd e retorna true se tiver
                                                      //identificador correspondente e false se não tiver
    public bool FecharJanela(IntPtr hWnd)
    {
        if (hWnd == IntPtr.Zero) //verifica se recebeu um identificador nulo
            return false;

        if (!IsWindow(hWnd)) //Verifica se o identificador ainda corresponde a uma janela existente
            return false;

        return PostMessage(hWnd, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
    }
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

    public void ListarClassesDasJanelas() //para identificar e diferenciar abertura no navegador e em modo aplicativo
    {
        EnumWindows((hWnd, lParam) => //solicita ao windows que percorra as janelas
        {
            if (!IsWindowVisible(hWnd)) //se a janela não tiver visível ignoramos ela
                return true;

            StringBuilder titulo = new StringBuilder(256); //Cria um buffer com capacidade inicial para 256 caracteres,
            StringBuilder classe = new StringBuilder(256); //no qual armazenaremos o título da janela

            GetWindowText(hWnd, titulo, titulo.Capacity); //Pede ao Windows que copie o título da janela identificada por hWnd
            GetClassName(hWnd, classe, classe.Capacity); //para o buffer titulo

            if (titulo.Length > 0) //verifica se obtemos um título não vazio
            {
                Console.WriteLine(
                    $"HWND: {hWnd} | Classe: {classe} | Título: {titulo}"
                );
            }

            return true; //Informa ao EnumWindows() que ele deve continuar percorrendo as demais janelas
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

    public HashSet<IntPtr> ObterJanelasChrome() //método que retorna um conjunto de identificadores de janelas
    {                                           //HashSet<T> é uma coleção do C# que não permite elementos duplicados.
        HashSet<IntPtr> janelas = new HashSet<IntPtr>();

        EnumWindows((hWnd, lParam) =>
        {
            if (!IsWindowVisible(hWnd))
                return true;

            StringBuilder classe = new StringBuilder(256);

            GetClassName(hWnd, classe, classe.Capacity); //consulta a classe da janela

            if (classe.ToString() != "Chrome_WidgetWin_1") //ignora as janelas de outras classes
                return true;

            GetWindowThreadProcessId(hWnd, out uint processId); //obtém o identificador do processo responsável pela janela

            try
            {
                using Process processo = Process.GetProcessById((int)processId); //O using garante que os recursos associados ao
                                                                                 //objeto sejam liberados ao sair do escopo.

                if (processo.ProcessName.Equals("chrome", StringComparison.OrdinalIgnoreCase))//Confirma que o processo é chrome
                {
                    janelas.Add(hWnd); //adiciona o identificador da janela ao conjunto
                }
            }
            catch (ArgumentException)
            {
                // O processo pode ter encerrado.
            }
            catch (InvalidOperationException)
            {
                // O processo pode não estar mais disponível.
            }

            return true;
        }, IntPtr.Zero);

        return janelas; //devolve todos os identificadores encontrados
    }

    public int ObterProcessoDaJanela(IntPtr hWnd) //método que recebe o hwnd(identificador de janela) e devolve o PID(id process)
    {
        if (hWnd == IntPtr.Zero || !IsWindow(hWnd)) //se o hwnd é nulo ou se o windows não reconhece hwnd como janela
            return 0;

        GetWindowThreadProcessId(hWnd, out uint processId); //consulta o windows pra saber qual processo responsável pela janela

        try
        {
            using Process processo = Process.GetProcessById((int)processId); //O using libera os recursos associados ao objeto 
                                                                             //quando ele deixa de ser utilizado.

            if (processo.ProcessName.Equals("chrome", StringComparison.OrdinalIgnoreCase)) //verifica se o processo é o chrome
            {
                return (int)processId; //devolve o PID encontrado
            }
        }
        catch (ArgumentException)
        {
            // O processo pode ter sido encerrado.
        }
        catch (InvalidOperationException)
        {
            // O processo pode não estar mais disponível.
        }

        return 0;
    }

    public bool JanelaChromeValida(IntPtr hWnd, int processIdEsperado)
    {
        if (processIdEsperado <= 0)
            return false;

        int processIdAtual = ObterProcessoDaJanela(hWnd);

        return processIdAtual == processIdEsperado;
    }
}