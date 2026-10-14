namespace ClapRoute.Detection;

public class ClapSequenceDetector //Contar as palmas e identificar o fim de uma sequência
{
    private int quantidadePalmas = 0; //armazena quantas palmas foram detectadas na sequência atual
    private long instanteUltimaPalma = 0; //guarda o momento da última palma válida
    private const int JANELA_SEQUENCIA_MS = 800; //define o intervalo máximo de espera entre palmas da mesma sequência
    private readonly object sincronizacao = new object(); //objeto utilizado para coordenar o acesso aos dados compartilhados
                                                          //readonly significa que não poderemos atribuir outro objeto a esse
                                                          //campo depois da inicialização exceto durante a construão
    public event Action? VSCodeReconhecido;
    public event Action? ChatGPTReconhecido;

    public void RegistrarPalma(long instanteAtual) //recebe o instante em ms no qual a palma foi reconhecida
    {
        lock (sincronizacao)//lock garante que somente uma thread por vez vá executar um bloco, a outra precisa aguardar até 
                            //que o bloqueio seja liberado.
        {
            quantidadePalmas++;
            instanteUltimaPalma = instanteAtual; //atualiza o momento da última palma
            Console.WriteLine($"Palmas registradas: {quantidadePalmas}");
        }
    }

    public void VerificarSequencia(long instanteatual)
    {
        lock (sincronizacao)
        {
            if (quantidadePalmas == 0)
            {
                return;
            }

            long tempoDecorrido = instanteatual - instanteUltimaPalma; //calcula a quanto tempo não reconhecemos outra palma

            if (tempoDecorrido >= JANELA_SEQUENCIA_MS)
            {
                if (quantidadePalmas == 2)
                {
                    Console.WriteLine("COMANDO RECONHECIDO: VS code");
                    VSCodeReconhecido?.Invoke();
                }
                else if (quantidadePalmas == 3)
                {
                    Console.WriteLine("COMANDO RECONHECIDO: ChatGPT");
                    ChatGPTReconhecido?.Invoke();

                }
                else
                {
                    Console.WriteLine("Sequência sem comando associado");
                }
                quantidadePalmas = 0; //reinicia o contador para a próxima sequência
            }
        }
    }
}