namespace CLapRoute.Configuration;

public class AppMenu
{
    private readonly List<string> aplicativos = new List<string> //Lista de aplicativos
    {
       "YouTube",
        "WhatsApp Web",
        "Instagram",
        "Notion",
        "Gemini",
        "Claude",
        "VS Code",
        "ChatGPT"
    };

    public string AplicativoDuasPalmas { get; private set; } = "";
    public string AplicativoTresPalmas { get; private set; } = "";
    //get: permitem consultar seu valor
    //set: permitem alterar seu valor somente dentro da classe AppMenu
    //inicializo as 2 variáveis como strings vazias 

    public void Configurar()
    {
        Console.WriteLine("==============================");
        Console.WriteLine("       MENU CLAPROUTE         ");
        Console.WriteLine("==============================");

        AplicativoDuasPalmas = EscolherAplicativo("Escolha o aplicativo para 2 palmas:", aplicativos); //listo os aplicativos

        List<string> opcoesRestantes = new List<string>(aplicativos); //crio uma nova lista

        opcoesRestantes.Remove(AplicativoDuasPalmas); //removo dessa nova lista o app q escolhi em 2 palmas

        AplicativoTresPalmas = EscolherAplicativo("Escolha o aplicativo para 3 palmas:", opcoesRestantes); //uso a lista - 1

        Console.WriteLine();
        Console.WriteLine("CONFIGURAÇÃO CONCLUÍDA!");
        Console.WriteLine($"2 palmas -> {AplicativoDuasPalmas}");
        Console.WriteLine($"3 palmas -> {AplicativoTresPalmas}");
    }

    private string EscolherAplicativo(string mensagem, List<string> opcoes)
    {
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine(mensagem); //mensagem mandando vc escolher

            for (int i = 0; i < opcoes.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {opcoes[i]}");  //percorro e exibo a lista de apps
            }

            Console.Write("\nDigite sua opção: ");

            string? entrada = Console.ReadLine(); //escolho um 

            if (int.TryParse(entrada, out int numero) && numero >= 1 && numero <= opcoes.Count)
            {
                //verifico se o usuário digitou um número válido, maior que 1 e dentro das opções disponíveis
                string escolhido = opcoes[numero - 1];
                Console.WriteLine($"Você escolheu: {escolhido}");
                return escolhido;
            }
            Console.WriteLine("Opção inválida. Tente novamente.");
        }
    }
}