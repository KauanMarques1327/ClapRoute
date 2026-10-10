using ClapRoute.Audio;
using NAudio.Wave;


Console.WriteLine("Microfones encontrados:");

for (int i = 0; i < WaveIn.DeviceCount; i++) //Device Count é uma propriedade estática da classe WaveIn que informa
                                             //quantos dispositivos de entrada de áudio o sistema disponibiliza 
{
    WaveInCapabilities dispositivo = WaveIn.GetCapabilities(i); //Variável capaz de armazenar informações do tipo WaveInCapabilities, 
                                                                //e coloque nela as informações do dispositivo de índice i 
                                                                //GetCapabilities é um método=quais informações possui o dispositivo x

    Console.WriteLine($"[{i}] {dispositivo.ProductName}"); //Product Name é um membro disponibilizado por WaveInCapabilities
                                                           //que fornece o nome do dispositivo
}


Console.WriteLine("====================");
Console.WriteLine("     ClapRoute      ");
Console.WriteLine("====================");
Console.WriteLine();
Console.WriteLine("2 palmas -> VsCode");
Console.WriteLine("3 palmas -> ChatGPT");
Console.WriteLine();
Console.WriteLine("ClapRoute iniciando...");

AudioCapture audio = new AudioCapture();

try
{
    Console.WriteLine("Prepare-se...");
    Thread.Sleep(2000); //faz a thread atual esperar 2 segundos

    audio.Iniciar();

    Console.WriteLine("Gravando por 4 segundos (primeiro segundo ignorado)");
    Thread.Sleep(4000);

}

finally
{
    audio.Parar();
    audio.Liberar();
}

audio.MostrarResultados();
Console.WriteLine("Captura encerrada. ");