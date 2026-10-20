using NAudio.Wave;
using System.Diagnostics;

namespace ClapRoute.Audio;

public class AudioCapture
{
    private readonly WaveIn microfone;
    private readonly Stopwatch cronometro = new Stopwatch();

    private double rmsAnterior = 0;
    private bool primeiraAmostra = true;

    private const double LIMIAR_VARIACAO = 0.15;
    private const int COOLDOWN_MS = 250; //O cooldown serve para evitar que uma única palma seja detectada várias vezes
    //Quando você bate uma palma, o microfone não recebe apenas uma amostra de áudio. Ele recebe vários blocos consecutivos, 
    //e o som pode provocar mais de um aumento brusco de RMS.

    private long instanteUltimaPalma = -COOLDOWN_MS;

    public event Action<long>? PalmaDetectada;

    public AudioCapture()
    {
        microfone = new WaveIn
        {
            WaveFormat = new WaveFormat(44100, 16, 1),
            BufferMilliseconds = 10
        };

        microfone.DataAvailable += AoReceberAudio;
    }

    private void AoReceberAudio(object? sender, WaveInEventArgs e)
    {
        if (cronometro.ElapsedMilliseconds < 1000) // Ignora o primeiro segundo de captura.
        {
            return;
        }

        double somaQuadrados = 0;
        int quantidadeAmostras = 0;

        for (int i = 0; i + 1 < e.BytesRecorded; i += 2) // Cada amostra de áudio possui 16 bits (2 bytes).
        {
            short amostra = BitConverter.ToInt16(e.Buffer, i);
            double amostraNormalizada = amostra / 32768.0; // Normaliza a amostra para aproximadamente -1 a 1.
            somaQuadrados += amostraNormalizada * amostraNormalizada;
            quantidadeAmostras++;
        }

        if (quantidadeAmostras == 0)
        {
            return;
        }

        double rms = Math.Sqrt(somaQuadrados / quantidadeAmostras); // Calcula o RMS do bloco de áudio.

        double variacaoRms = 0;

        if (!primeiraAmostra)
        {
            variacaoRms = rms - rmsAnterior;
        }

        rmsAnterior = rms;
        primeiraAmostra = false;

        if (variacaoRms > LIMIAR_VARIACAO) // Detecta aumentos bruscos na amplitude efetiva do áudio.
        {
            long instanteAtual = cronometro.ElapsedMilliseconds;

            if (instanteAtual - instanteUltimaPalma >= COOLDOWN_MS) // Evita registrar palmas muito próximas.
            {
                Console.WriteLine("PALMA DETECTADA!");
                instanteUltimaPalma = instanteAtual;
                PalmaDetectada?.Invoke(instanteAtual);
            }
        }
    }

    public long ObterTempoDecorrido()
    {
        return cronometro.ElapsedMilliseconds;
    }

    public void Iniciar()
    {
        rmsAnterior = 0; // Reinicia o estado da detecção para uma nova captura.
        primeiraAmostra = true;
        instanteUltimaPalma = -COOLDOWN_MS;

        cronometro.Restart();
        microfone.StartRecording();
    }

    public void Parar()
    {
        microfone.StopRecording();
    }

    public void Liberar()
    {
        microfone.DataAvailable -= AoReceberAudio;
        microfone.Dispose();
    }
}