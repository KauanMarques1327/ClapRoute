using NAudio.Wave; //Usar as classes que estão dentro da Pasta Wave da biblioteca NAudio 

namespace ClapRoute.Audio;

public class AudioCapture
{
    private WaveIn microfone; //declaração

    private void AoReceberAudio(object? sender, WaveInEventArgs e)
    {
        Console.WriteLine($"Recebidos: {e.BytesRecorded} bytes");
    }
    public AudioCapture()
    {
        microfone = new WaveIn(); //instanciação
        microfone.WaveFormat = new WaveFormat(44100, 16, 1); // (sample/seg, bits/sample, channel(captura mono))
        microfone.DataAvailable += AoReceberAudio; //quando o mic disponibilizar novos dados, execute meu método
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