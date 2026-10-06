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
Console.WriteLine("1 palma -> Notion");
Console.WriteLine("2 palmas -> VsCode");
Console.WriteLine("3 palmas -> ChatGPT");
Console.WriteLine();
Console.WriteLine("ClapRoute iniciando...");

AudioCapture audio = new AudioCapture();

try
{
    audio.Iniciar();
    Console.WriteLine("Microfone iniciado!");
    Console.WriteLine("Pressione ENTER para encerrar");
    Console.ReadLine();
}

finally
{
    audio.Parar();
    audio.Liberar();
}

Console.WriteLine("Captura encerrada. ");