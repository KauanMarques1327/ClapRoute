👏 ClapRoute
Controle aplicativos e sites por sequências de palmas usando C# e .NET.

O ClapRoute é um projeto pessoal desenvolvido para explorar captura e processamento de áudio em tempo real, Programação Orientada a Objetos (POO) e Programação Orientada a Eventos. A aplicação monitora o microfone do computador, identifica sequências de palmas e executa ações configuradas pelo usuário.
Status: projeto de estudos em desenvolvimento. O reconhecimento pode variar conforme o microfone, a distância, o volume das palmas e os ruídos do ambiente.


✨ Funcionalidades
- 🎤 Captura de áudio pelo microfone com a biblioteca NAudio.
- 👏 Reconhecimento de sequências de duas ou três palmas.
- ⚙️ Menu para escolher qual aplicativo ou site associar a cada sequência.
- 🖥️ Abertura e solicitação de fechamento de aplicativos
- 🌐 Abertura de sites em janelas do Google Chrome, usando o modo aplicativo (--app).
- ⏱️ Janela de espera para confirmar a sequência e evitar executar o comando de duas palmas antes de uma possível terceira.

🧰 Tecnologias
Linguagem de programação principal: C#
Plataforma de execução: .NET
Captura das amostras de áudio do microfone: Biblioteca NAudio
Estimativa da amplitude efetiva do sinal em pequenas janelas: RMS (Root Mean Square)
Programação Orientada a Objetos
Programação Orientada a Eventos


🔍 Como funciona?
O microfone captura o áudio, ocorre o cálculo de RMS e a detecção de possível palma, calcula o intervalo entre as detecções de palmas de tempo entre uma janela e outra pra ver se vai ter sequência ou não, após o fim da sequência se forem registrados 2 ou 3 palmas um comando de abertura ou fechamento de uma página (em modo aplicativo) ou de um aplicativo será executada de acordo com o status. 


Sobre o uso do RMS:
Essa abordagem pode reduzir a influência de oscilações isoladas, tentando eliminar ao máximo os falsos positivos
O RMS (Root Mean Square) calcula a amplitude efetiva de um sinal considerando um conjunto de amostras:
RMS = √[(x₁² + x₂² + ... + xₙ²) / n]
Em vez de observar apenas a maior amplitude de uma amostra (peak), o RMS oferece uma medida da intensidade do sinal ao longo de uma pequena janela de tempo. Isso pode reduzir a influência de oscilações isoladas, dependendo do tamanho da janela.
O pico considera apenas a maior amostra, RMS considera todas. O RMS tem menor sensibilidade a uma janela isolada.


🏗️ Organização do código
Program.cs: inicializa os componentes e conecta os eventos
AudioCapture.cs: captura e analisa o áudio do microfone
ClapSequenceDetector.cs: agrupa as palmas e reconhece sequências
AppMenu.cs: permite configurar os comandos de duas e três palmas
CommandExecutor.cs: executa as ações associadas aos comandos
WindowManager.cs: auxilia no gerenciamento de janelas no Windows


🚀 Como executar
Pré-requisitos:
- Windows;
- .NET 10 SDK instalado;
- Microfone conectado e autorizado nas configurações de privacidade do Windows;
- Google Chrome para os comandos que abrem sites;
- Aplicativos correspondentes instalados, caso você escolha comandos para programas locais.

Passo a passo:
1. Clone o repositório:
   git clone https://github.com/KauanMarques1327/ClapRoute.git
2. Acesse a pasta do projeto: cd ClapRoute
3. Restaure as dependências e execute a aplicação a partir da pasta que contém o arquivo .csproj:
   dotnet restore
   dotnet run
4. No menu do terminal, escolha o aplicativo ou site associado a duas palmas e a três palmas.
5. Com o programa em execução, faça uma das sequências para testar o comando.
Se o arquivo .csproj estiver em uma subpasta, entre nela antes de executar os comandos. Substitua KauanMarques1327 pelo seu nome de usuário do GitHub quando publicar o repositório.

⚠️ Limitações conhecidas
- O reconhecimento de palmas pode apresentar falsos positivos ou deixar de detectar eventos em ambientes barulhentos.
- O desempenho depende da sensibilidade do microfone e dos parâmetros de detecção.
- A identificação de janelas do Chrome por identificadores do Windows é uma abordagem experimental; o gerenciamento dessas janelas ainda pode precisar de melhorias para evitar ambiguidades com outras janelas do navegador.
- O projeto foi pensado para Windows e utiliza recursos específicos desse sistema operacional.

🎯 Objetivo do projeto
O ClapRoute nasceu como uma forma de colocar em prática os conhecimentos adquiridos durante meus estudos de C#, iniciados e aprofundados a partir da experiência com desenvolvimento em Unity e projetos de Realidade Virtual (VR) no laboratório Huawei Academy do IFCE. Mais do que automatizar tarefas, o projeto representa uma oportunidade de estudar arquitetura de software, eventos, processamento de sinais e integração com o sistema operacional. Desenvolvido como projeto pessoal de aprendizado. 🚀