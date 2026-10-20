namespace ClapRoute.Detection;

public class ClapSequenceDetector //Contar as palmas e identificar o fim de uma sequência
{
    private int quantidadePalmas = 0; //armazena quantas palmas foram detectadas na sequência atual
    private long instanteUltimaPalma = 0; //guarda o momento da última palma válida
    private const int JANELA_SEQUENCIA_MS = 800; //define o intervalo máximo de espera entre palmas da mesma sequência
    private readonly object sincronizacao = new object(); //objeto utilizado para coordenar o acesso aos dados compartilhados
                                                          //readonly significa que não poderemos atribuir outro objeto a esse
                                                          //campo depois da inicialização exceto durante a construão
    public event Action? DuasPalmasReconhecidas;
    public event Action? TresPalmasReconhecidas;

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
        int comandoReconhecido = 0;
        lock (sincronizacao)
        {
            if (quantidadePalmas == 0)
            {
                return;
            }

            long tempoDecorrido = instanteatual - instanteUltimaPalma; //calcula a quanto tempo não reconhecemos outra palma

            if (tempoDecorrido < JANELA_SEQUENCIA_MS)
            {
                return;
            }
            comandoReconhecido = quantidadePalmas;
            quantidadePalmas = 0;
        }

        if (comandoReconhecido == 2)
        {
            DuasPalmasReconhecidas?.Invoke();
        }
        else if (comandoReconhecido == 3)
        {
            TresPalmasReconhecidas?.Invoke();

        }
        else
        {
            Console.WriteLine("Sequência sem comando associado");
        }
    }
}