# NutriAR

NutriAR é um aplicativo de orientação nutricional que usa a câmera do celular para ler o código de barras de um produto e mostrar, na hora, quantas calorias e nutrientes aquela porção tem. A ideia nasceu de um problema bem comum: rótulos nutricionais são pequenos, cheios de números e difíceis de interpretar rápido, seja no mercado ou na mesa. O app tenta resolver isso traduzindo o rótulo em uma informação simples e visual.

É um protótipo desenvolvido em **Unity 6000.0f1**, pensado primeiro para celular, em orientação vertical.

## Como funciona

O uso do app segue um fluxo direto:

1. **Escanear** — o usuário aponta a câmera para o código de barras (EAN-13) do produto.
2. **Validar** — o app confere se o código lido é válido, checando o dígito verificador.
3. **Consultar** — o código é usado para buscar o produto na base aberta [Open Food Facts](https://world.openfoodfacts.org/), que retorna informações nutricionais.
4. **Ajustar a porção** — o usuário informa quanto vai comer, em incrementos de meia porção, e os valores de calorias e macronutrientes são recalculados automaticamente.
5. **Registrar** — a porção consumida é somada ao total de calorias do dia, que fica salvo no próprio aparelho.

Se a câmera não conseguir ler o código — por falta de luz, embalagem amassada, etc. — o usuário pode digitar o número manualmente e seguir o mesmo fluxo.

## O que o app mostra

Para cada produto, o NutriAR exibe energia, carboidratos, proteínas, gorduras, fibras e açúcares (quando esses dados estão disponíveis na base consultada). Além dos números, ele também traz:

- **Alertas nutricionais**, baseados nos limites de referência da rotulagem frontal da Anvisa, sinalizando quando o produto é alto em açúcares adicionados, gordura saturada ou sódio.
- **Uma orientação curta e educativa** junto ao resultado, para ajudar a interpretar o que aquilo significa na prática.
- **Um diário do dia**, comparando o total de calorias já registradas com uma referência geral de 2.000 kcal — apresentada como parâmetro geral, não como meta individual de ninguém.

Quando o produto não é encontrado na base, a conexão falha ou faltam dados de calorias, o app avisa claramente em vez de mostrar informação incompleta ou inventada.

## Funcionalidades

- Leitura de código de barras pela câmera traseira do celular (ou webcam, quando testado direto no Editor do Unity).
- Leitor de EAN-13 próprio, sem depender de bibliotecas externas de scanner.
- Consulta somente de leitura ao Open Food Facts — o app não envia nem altera dados na base, só lê (exige conexão com a internet).
- Entrada manual de código de barras, como alternativa à câmera.
- Ajuste de porção de meia em meia, até cinco porções.
- Registro diário de calorias, salvo localmente no aparelho.
- Catálogo de exemplos rápidos (banana, refrigerante, barra de cereal, pão de queijo) para testar o app sem precisar escanear um produto de verdade.

## Privacidade

Todo o diário alimentar fica salvo **localmente**, no próprio celular. Nenhuma informação de consumo é enviada a um servidor — a única comunicação externa do app é a consulta de produtos ao Open Food Facts pelo código de barras.

## Como rodar o projeto

1. Instale o **Unity Hub** e, por ele, o **Unity 6000.0f1**, incluindo o módulo de build para Android.
2. Clone este repositório e abra a pasta pelo Unity Hub.
3. Para testar rápido, dê Play no próprio Editor — a webcam do computador funciona como câmera.
4. Para instalar no celular: ative a depuração USB no Android, conecte o aparelho, mude a plataforma de build para Android em *File > Build Profiles* e use *Build And Run*.

O app precisa de internet em uso, já que a consulta de produtos é feita em tempo real no Open Food Facts.

## Limitações atuais e próximos passos

Este é um protótipo, e alguns pontos da proposta original ainda não foram implementados:

- **Leitura da tabela nutricional impressa (OCR):** hoje o app identifica o produto pelo código de barras, não lendo diretamente o texto da embalagem.
- **Reconhecimento de alimentos sem embalagem:** existe só um catálogo de exemplos; o app ainda não reconhece um alimento a partir de uma foto.
- **Realidade aumentada espacial:** a tela atual mostra a câmera com uma moldura de leitura; a ideia de ancorar cartões de informação no ambiente via AR Foundation é um próximo passo.

## Créditos

As informações nutricionais vêm do [Open Food Facts](https://world.openfoodfacts.org/), uma base de dados aberta e mantida por colaboração da comunidade. Antes de distribuir o app publicamente, vale revisar os [termos de uso da API](https://openfoodfacts.github.io/documentation/docs/Product-Opener/api/).
