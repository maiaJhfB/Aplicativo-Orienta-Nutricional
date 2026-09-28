# NutriAR — orientação nutricional em realidade aumentada

Protótipo para **Unity 6000.0f1**, pensado primeiro para celular em orientação vertical. O app lê códigos EAN-13 pela câmera, consulta informações do produto no Open Food Facts, permite revisar a porção, exibe calorias e macronutrientes e registra as calorias informadas no dia.

## Requisitos e estado atual

| Requisito da proposta | Estado |
| --- | --- |
| Apontar a câmera a um rótulo | Lê códigos de barras EAN-13 pela câmera traseira ou webcam; consulta o produto pela internet. |
| Alternativa quando não se consegue escanear | Campo para digitar um EAN-13 válido e consultar o mesmo banco. |
| Estimar calorias e macronutrientes | Mostra energia, carboidratos, proteínas, gorduras, fibras e açúcares quando a base possui esses dados. |
| Considerar quantidade consumida | Ajuste da porção em incrementos de meia porção; valores da tela são recalculados. |
| Entender calorias a mais | Botão registra a porção no total de hoje e mostra a diferença em relação a uma referência geral de 2.000 kcal. |
| Alertas nutricionais | Sinaliza os limites da rotulagem frontal da Anvisa quando há dados de açúcares adicionados, gordura saturada ou sódio; diferencia açúcares totais. |
| Orientação breve | Exibe uma sugestão educativa com o resultado nutricional. |
| Alimentos sem embalagem | Catálogo de exemplos para demonstração; reconhecimento da imagem do alimento ainda não implementado. |
| Ler a tabela nutricional impressa | OCR da tabela ainda não implementado; neste protótipo, o rótulo é associado pelo código de barras. |
| Realidade aumentada espacial | A interface mostra a câmera e uma moldura; cartões ancorados ao ambiente via AR Foundation ainda não implementados. |

## O que funciona

- Câmera traseira no celular e webcam no Editor, com permissão solicitada ao usuário.
- Leitura EAN-13 sem uma dependência de scanner adicional e validação do dígito verificador.
- Consulta somente de leitura ao produto e nutrientes no Open Food Facts; exige internet.
- Entrada manual do código quando a leitura da câmera falhar.
- Ajuste da quantidade de meia porção até cinco porções e atualização de calorias/macros.
- Registro local das calorias por dia no aparelho; não envia diário alimentar ao servidor.
- Referência de 2.000 kcal apresentada como referência geral, não como meta individual.
- Exemplos rápidos: banana, refrigerante, barra de cereal e pão de queijo.
- Aviso quando o produto está ausente da base, a conexão falha ou os dados de calorias não estão disponíveis.


O Open Food Facts é uma base aberta mantida por contribuições comunitárias. Este projeto consulta o endpoint público de produtos por código de barras e exibe uma indicação da origem. Consulte os [termos e orientações da API](https://openfoodfacts.github.io/documentation/docs/Product-Opener/api/) antes de distribuir o app.
