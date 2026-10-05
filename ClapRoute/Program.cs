using ClapRoute.Audio;


Console.WriteLine("====================");
Console.WriteLine("     ClapRoute      ");
Console.WriteLine("====================");
Console.WriteLine();
Console.WriteLine("1 palma -> Notion");
Console.WriteLine("2 palmas -> VsCode");
Console.WriteLine("1 palma -> ChatGPT");
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