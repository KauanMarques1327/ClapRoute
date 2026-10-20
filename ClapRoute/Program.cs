using ClapRoute.Audio;
using ClapRoute.Detection;
using ClapRoute.Commands;
using CLapRoute.Configuration;
using NAudio.Wave;
using System.DirectoryServices.ActiveDirectory;

Console.WriteLine("====================");
Console.WriteLine("     ClapRoute      ");
Console.WriteLine("====================");
Console.WriteLine();
Console.WriteLine("ClapRoute iniciando...");

AudioCapture audio = new AudioCapture();
ClapSequenceDetector detector = new ClapSequenceDetector();
CommandExecutor executor = new CommandExecutor();
AppMenu menu = new AppMenu();

// ================================================ CONFIGURAÇÃO =============================================================//

menu.Configurar();
Console.WriteLine();

//=======================Referências a métodos que serão usados futuramente e não agora========================================//


audio.PalmaDetectada += detector.RegistrarPalma; //quando o objeto audio detectar uma palma, execute o método RegistrarPalma
                                                 //do objeto detector   

detector.DuasPalmasReconhecidas += () => //()=> representa uma expressão lambda que significa que podemos fazer com que o evento
                                         //execute uma função que consulte uma escolha armazenada no menu
{
    Console.WriteLine($"COMANDO RECONHECIDO: {menu.AplicativoDuasPalmas}");
    executor.ExecutarAplicativo(menu.AplicativoDuasPalmas);
};

detector.TresPalmasReconhecidas += () =>
{
    Console.WriteLine($"COMANDO RECONHECIDO: {menu.AplicativoTresPalmas}");
    executor.ExecutarAplicativo(menu.AplicativoTresPalmas);
};

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