using NAudio.Wave; //Usar as classes que estão dentro da Pasta Wave da biblioteca NAudio 

namespace ClapRoute.Audio;

public class AudioCapture
{
    private WaveIn microfone; //declaração

    private void AoReceberAudio(object? sender, WaveInEventArgs e)
    {
        double somaQuadrados = 0;
        int quantidadeAmostras = 0;
        for (int i = 0; i < e.BytesRecorded; i += 2) //i+=2 pois cada amostra ocupa 2 bytes
        {
            short amostra = BitConverter.ToInt16(e.Buffer, i); //Pegamos 2 bytes do buffer, começando na posição i e 
                                                               //interprete-os como inteiro de 16 bits com sinal (short)

            double amostraNormalizada = amostra / 32768.0; //Normaliza as amostras de 16 bits para a faixa -1 +1 0

            somaQuadrados += amostraNormalizada * amostraNormalizada;

            quantidadeAmostras++;
        }

        double rms = Math.Sqrt(somaQuadrados / quantidadeAmostras);
        Console.WriteLine($"Intensidade: {rms:f8}");
    }
    public AudioCapture()
    {
        microfone = new WaveIn(); //instanciação
        microfone.WaveFormat = new WaveFormat(44100, 16, 1); // (sample/seg, bits/sample, channel(captura mono))
        microfone.DataAvailable += AoReceberAudio;           //quando o mic disponibilizar novos dados, execute meu método
                                                             //não usamos AoReceberAudio() porque não queremos usar imediatamente
    }

    public void Iniciar()
    {
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