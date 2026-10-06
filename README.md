# NutriAR

NutriAR é um aplicativo de orientação nutricional que usa a câmera do celular para ler o código de barras de um produto e mostrar, na hora, quantas calorias e nutrientes aquela porção tem. A ideia nasceu de um problema bem comum: rótulos nutricionais são pequenos, cheios de números e difíceis de interpretar rápido, seja no mercado ou na mesa. O app tenta resolver isso traduzindo o rótulo em uma informação simples e visual.

## Demonstração

![Demonstração do projeto](assets/demo.gif)

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



