using NAudio.Wave; //Usar as classes que estão dentro da Pasta Wave da biblioteca NAudio
using System.Diagnostics;

namespace ClapRoute.Audio;

public class AudioCapture
{
    private WaveIn microfone; //declaração
    double maiorRms = 0;
    double maiorPico = 0;
    private double rmsAnterior = 0;
    private double maiorVariacaoRms = 0;
    private Stopwatch cronometro = new Stopwatch();
    private long instanteMaiorPico = 0;
    private bool primeiraAmostra = true;
    private const double LIMIAR_VARIACAO = 0.15; //porque está nas faixas que medimos, mas não significa que é o valor ideal

    private const double LIMIAR_PICO = 0.10; //considere valores acima de 0,01 como canidatos a sons fortes
    private const int COOLDOWN_MS = 250; //precisamos esperar pelo menos 250 milissegundos entre duas detecções.
    private long instanteUltimaPalma = -COOLDOWN_MS; //armazena o instante da última palma reconhecida, inicializamos dessa
                                                     //dessa forma pois ainda não houve nenhuma palma, assim a primeira
                                                     //execução não é bloqueada por cooldown inexistente        

    private void AoReceberAudio(object? sender, WaveInEventArgs e)
    {
        if (cronometro.ElapsedMilliseconds < 1000) //Se ainda não passou um segundo desde o início da captura, 
                                                   //ignore esse bloco de áudio
        {
            return;
        }

        double somaQuadrados = 0;
        int quantidadeAmostras = 0;
        double picoMaximo = 0;
        for (int i = 0; i < e.BytesRecorded; i += 2) //i+=2 pois cada amostra ocupa 2 bytes
        {
            short amostra = BitConverter.ToInt16(e.Buffer, i); //Pegamos 2 bytes do buffer, começando na posição i e 
                                                               //interprete-os como inteiro de 16 bits com sinal (short)

            double amostraNormalizada = amostra / 32768.0; //Normaliza as amostras de 16 bits para a faixa -1 +1 0
            double amplitude = Math.Abs(amostraNormalizada); //Math.abs é uma classe que retorna o valor absoluto de um número

            if (amplitude > picoMaximo)
            {
                picoMaximo = amplitude;
            }


            somaQuadrados += amostraNormalizada * amostraNormalizada;

            quantidadeAmostras++;
        }

        double rms = Math.Sqrt(somaQuadrados / quantidadeAmostras);

        double variacaoRms = 0;
        if (!primeiraAmostra)
        {
            //No primeiro bloco válido apenas armazenamos o RMS, a partir do segundo começamos a calcular a variação
            variacaoRms = rms - rmsAnterior;
        }
        rmsAnterior = rms;
        primeiraAmostra = false;

        if (rms > maiorRms) //registra o maior RMS (a maior intensidade média registrada em um bloco)
        {
            maiorRms = rms;
        }

        if (picoMaximo > maiorPico) //registra o maior pico de amplitude (a maior amplitude instantânea registrada)
        {
            maiorPico = picoMaximo;
            instanteMaiorPico = cronometro.ElapsedMilliseconds; //retorna o tempo total decorrido em milissegundos desde que a
                                                                //que a contagem foi iniciada
        }

        if (variacaoRms > maiorVariacaoRms) //o maior aumento de RMS (maior aumento de intensidade entre 2 blocos consecutivos )
        {
            maiorVariacaoRms = variacaoRms;
        }

        if (variacaoRms > LIMIAR_VARIACAO)
        {
            long instanteAtual = cronometro.ElapsedMilliseconds; //quantos milissegundos passaram desde o inicio do cronômetro

            if (instanteAtual - instanteUltimaPalma >= COOLDOWN_MS) //ja se passaram pelo menos 250ms desde a última palma?
            {
                Console.WriteLine("PALMA DETECTADA!");
                instanteUltimaPalma = instanteAtual; //atualiza a referência temporal para que o cooldown seja contado a partir
                                                     //da última detecção válida
            }
        }
    }

    public void MostrarResultados()
    {
        Console.WriteLine($"Maior RMS: {maiorRms:f8}");
        Console.WriteLine($"Maior pico: {maiorPico:f8}");
        Console.WriteLine($"Maior variação RMS: {maiorVariacaoRms:f8}");
        Console.WriteLine($"Instante do maior pico: {instanteMaiorPico} ms");
    }
    public AudioCapture()
    {
        microfone = new WaveIn(); //instanciação
        microfone.WaveFormat = new WaveFormat(44100, 16, 1); // (sample/seg, bits/sample, channel(captura mono))
        microfone.BufferMilliseconds = 10; //configura a duração solicitada para os buffers de captura em ms
        microfone.DataAvailable += AoReceberAudio;//quando o mic disponibilizar novos dados, execute meu método
                                                  //não usamos AoReceberAudio() porque não queremos usar imediatamente
    }

    public void Iniciar()
    {
        cronometro.Restart(); //reinicia o cronômetro do zero e começa a contagem
        microfone.StartRecording();
    }

    public void Parar()
    {
        microfone.StopRecording();
    }

    public void Liberar()
    {
        microfone.DataAvailable -= AoReceberAudio;
        microfone.Dispose(); //Libera memória manualmente sendo um reforço ao Garbage Collector
    }
}