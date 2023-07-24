## Projeto

Esse sistema foi construído visando atender aos requisitos propostos no desafio BT da Inoa. Seu objetivo é ser uma aplicação de console capaz de monitorar de forma contínua um determinado ativo da B3 e avisar via e-mail caso a sua cotação varie para além de um determinado intervalo de valores, aconselhando a compra caso seu valor caia abaixo de um determinado número ou a venda caso suba acima de outro. 

## Tecnologias

O código do projeto foi desenvolvido interaimente em `.NET 7.0`, podendo ser portado e executado em diferentes sistemas operacionais. Para auxiliar o desenvolvimento, foi utilizado como IDE o `Visual Studio Code`, por se tratar de uma ferramenta com diversos recursos e funcionalidades variadas para o desenvolvimento. 

## Serviços externos e integrações

- Para a consulta das cotações dos ativos na B3, foi utilizada a API fornecida pela **BRAPI**, cuja documentação está disponível pelo link https://brapi.dev. Ela foi selecionada por ser uma ferramenta gratuita, com bom tempo de resposta e que pode ser integrada de forma simples e prática ao sistema. Aĺém disso, ela fornece outros dados que podem ser relevantes para o monitoramento de um determinado ativo a longo prazo, o que pode ser útil casa haja a expansão desse projeto no futuro. 

- Já para possibilitar o envio de e-mails, foram utilizadas bibliotecas nativas da própria linguagem, de forma a deixar configurável e permitir o envio por diversos provedores. Nos testes foi utilizado a plataforma `Gmail` para esse disparo, estando os seus dados base já presentes no arquivo de configurãção (como host e porta).

## Arquitetura

A arquitetura desse projeto foi contruída tendo em mente os principais requisitos propostos no desafio. Por isso, trata-se de uma aplicação de console, a ser executada recebendo três argumentos iniciais indicando qual o ativo a ser monitorado e quais os valores que devem ser usados como parâmetro do envio da comunicação. Além disso, ele possui um arquivo de configuração próprio que detalha qual o servidor de SMTP a ser utilizado, assim como os endereços de origem e destino, e outros que permitem a personalização de alguns comportamentos do sistemas (a serem descritos na seção abaixo).

A organização do código desenvolvido se encontra dividio em três seções:

- A primeira inclui os arquivos que estão na pasta raiz e que funcionam como a base do serviço. Nela estão o arquivo de configuração `appsettings` no formato json e o `Program` que funciona como ponto de entrada para a execução do projeto. Nesse arquivo está toda a lógica de leitura dos argumentos passados na chamada, a estrutura de leitura das configurações (podendo ser via arquivo ou variáveis de ambiente) e o fluxo de controle de repetição do projeto, incluindo o tratamento de possíveis erros de configuração e permitindo que o monitoramento do valor ocorra em ciclos de intervalo fixo, abortando caso ocorram muitos erros.
- Na pasta `models` está a definição de algumas classes que são utilizadas ao longo da execução da aplicação. Todas as estruturas definidas nessa seção já contam com as validações básicas que são necessárias na hora de criação dos objetos, assim como alguns métodos para executar operações em cima deles. Nela temos o arquivo `MailConfig`, que representa os parâmetros usados para o disparo dos e-mails (como configurações do servidor SMTP ou dados como e-mail de origem, destino, senha para autenticação, etc) e o arquivo `Stock` que representa o ativo que está sendo monitorado, incluindo dados como o seu nome ou valores de margem, assim como operações básicas de comparação.
- Por fim, na pasta `services` se encontram os arquivos contendo as lógicas individuais e o funcionamento dos principais componentes do sistema. Primeiramente, temos o `MailService` que agrega a instanciação do cliente SMTP e disponibiliza a função que permite o envio das mensagens de aviso, incluindo toda a composição do assunto e do corpo principal da mensagem. Em seguida temos o `QueryService` que é responsável pela comunicação com a API de cotação de valores para um determinado ativo, além de incluir a instanciação do cliente HTTP que realizará a comunicação e a interpretação do resultado retornado na chamada feita, extraindo e transformando o valor reebido no dado a ser comparado. Por fim, temos o `MonitorService` que é chamado diretamente pela função `main` do projeto e que atua de forma a orquestrar os demais componentes, recebendo a resposta da cotação mais atual da ação, comparando com os valores recebidos e requisitando o envio do e-mail com a comunicação com rota (Compra ou venda do ativo).

## Envs

As seguintes variáveis estão disponíveis para a configuração da execução do projeto. Elas estão divididas em três seções diferentes de acordo com a sua natureza  e podem ser definidas tanto via arquivo de configuração (`appsettings.json`) ou via variáveis de ambiente do sistema, sendo que algumas delas são obrigatórias enquanto outras, se não forem definidas, irão usar valores padrões:


### Variáveis de comportamento do projeto

- **SleepTime** `Int`: Tempo, em segundos, que o sistema deve aguardar após encerrar um ciclo de processamento e consulta da cotação. Opcional, se não passado irá usar 60s como padrão;
- **MaxErrorsToAbort** `Int`: Quantidade máxima de erros que podem ocorrer nos ciclos de consulta antes do sistema abortar a execução. Cada ciclo de execução problemático irá contar como uma ocorrência no agregador. Opcional, se não passado não irá abortar a execução independente de quantos erros venham a ocorrer;
- **ResetMaxErrors** `Boolean`: Se deve ou não recomeçar a contagem de erros após um ciclo de execução sem erro. Opcional, se não passado irá usar um valor padrão `false` e o contador de erros não irá ser reiniciado, agregando a contagem de erros desde o início do programa.

## Variáveis de comportamento da API de consulta

- **APIConfig**: Seção que inclui as configurações necessárias para a execução da consulta na API externa de cotações de ativos. No momento não foi definido nenhum campo, mas pode ser usada caso no futuro seja necessário uma chave ou credencial para acessar a mesma.

## Variáveis de comportamento do serviço de email

Se encontram dentro da seção **MailConfig** e inclui:

- **Host** `String`: O endereço do servidor SMTP a ser utilizado. Obrigatório e o valor já presente representa o serviço disponibilizado pelo `Gmail`;
- **Port** `Int`: A porta do servidor SMTP a ser utilizado. Obrigatório e o valor já presente representa a que deve ser utilizada no caso do `Gmail`;
- **FromEmail** `String`: O endereço de origem pelo qual o e-mail deve ser disparado. Obrigatório.
- **Password** `String`: A senha para acessar o endereço de e-mail definido acima. Obrigatório e preferencialmente deve ser passado de forma a manter o valor secreto (via Secrets ou variáveis de ambiente).
- **ToEmail** `String`: Endereço de e-mail de destino para qual a mensagem final deve ser enviada. Obrigatório.
- **CcEmail** `List<String>`: Lista de outros endereços de e-mail que devem receber a mensagem em cópia. Opcional e caso necessário deve ser usado em um formato de lista, com cada item contendo um único endereço.

**Obs**: No caso de contas do Gmail, pode ser necessário a ativação da autenticação em dois fatores e a obtenção de uma senha específica para o uso em apps, Caso contrário podem vir a ocorrer problemas na autenticação com o servidor de e-mail.

## Instalação e execução

Para a correta execução do projeto é necesário primeiro a instalação do runtime do `dotnet` referente a versão 7.0 ou superior. Em seguida podem ser feitos os seguintes passo:

- Primeiro abra um terminal, selecione uma pasta e clone o projeto:

```
git clone git@github.com:ptmarmello/Inoa-challenge.git
```

- Acesse o diretório raiz do projeto:
```
cd Inoa
```

- Atualize os valores de configuração presente no arquivo `appsettings.json`;

- Execute o programa usando o seguinte comando e passando os argumentos na chamada para execução:
```
dotnet run <Nome> <ValorMin> <ValorMax>
```

*Exemplo*: 
```
dotnet run PETR4 30.00 30.50
```

- Caso prefira, estão disponíveis duas versões do código já compilados para sistemas operacionais windows e linux dentro da pasta Relase. Basta apenas acessar a pasta correspondente, atualizar o arquivo de configuração local e executar via terminal o arquivo `Inoa.exe` no primeiro caso ou o arquivo `Inoa.dll` no segundo, passando os devidos parâmetros.

*Exemplo no Windows*: 
```
cd ./Release/win-x64
./Inoa PETR4 30.00 30.50
```

*Exemplo no Linux*: 
```
cd ./Release/win-x64
dotnet Inoa.dll PETR4 30.00 30.50
```
