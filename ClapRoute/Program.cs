using ClapRoute.Audio;
using ClapRoute.Detection;
using ClapRoute.Commands;
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
ClapSequenceDetector detector = new ClapSequenceDetector();
CommandExecutor executor = new CommandExecutor();

//=======================Referências a métodos que serão usados futuramente e não agora========================================//


audio.PalmaDetectada += detector.RegistrarPalma; //quando o objeto audio detectar uma palma, execute o método RegistrarPalma
                                                 //do objeto detector   

detector.VSCodeReconhecido += executor.AbrirVSCode; //quando o objeto detector disparar o evento VSCodeReconhecido, execute o 
                                                    //método AbrirVSCode() do objeto executor

detector.ChatGPTReconhecido += executor.AbrirChatGPT;

//============================================================================================================================//


try
{
    Console.WriteLine("Prepare-se...");
    Thread.Sleep(2000); //faz a thread atual esperar 2 segundos

    audio.Iniciar();
    Thread.Sleep(1000);

    Console.WriteLine("CLAPROUTE PRONTO! Pode bater palmas");

    while (true) //enquanto o cronômetro indicar menos que 4seg continue executando o laço
    {
        detector.VerificarSequencia(audio.ObterTempoDecorrido());

        if (Console.KeyAvailable)
        {
            ConsoleKeyInfo tecla = Console.ReadKey(intercept: true);

            if (tecla.Key == ConsoleKey.Enter)
            {
                Console.WriteLine("Encerrando ClapRoute...");
                break;
            }
        }
        Thread.Sleep(20);
    }

}

finally
{
    audio.Parar();
    audio.Liberar();
}

Console.WriteLine("Captura encerrada. ");