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

## Como abrir

1. No Unity Hub, clique em **Add project from disk** e selecione esta pasta.
2. Abra usando o Editor **6000.0.0f1**.
3. Aguarde a instalação dos pacotes na primeira abertura.
4. Se a cena ainda não estiver criada, use o menu **NutriAR > Preparar projeto**.
5. Abra `Assets/Scenes/NutriAR.unity` e pressione **Play**.

A interface é criada em tempo de execução, então a cena permanece pequena e fácil de manter.

## Teste no celular

### Android

1. Instale o módulo Android pelo Unity Hub.
2. Em **File > Build Profiles**, escolha Android e adicione a cena `NutriAR`.
3. Conecte um aparelho com depuração USB e use **Build and Run**.
4. Autorize a câmera quando o sistema solicitar.

### iOS

1. Instale o módulo iOS pelo Unity Hub.
2. Troque o perfil de build para iOS e gere o projeto Xcode.
3. Configure a assinatura no Xcode e execute em um iPhone.

## Escopo e segurança

Os números são estimativas educativas fixas para demonstrar a experiência. O protótipo **não identifica alimentos de verdade**, não interpreta rótulos e não oferece diagnóstico. Quantidades reais variam por receita, marca e porção; o usuário deve conferir o rótulo e procurar um nutricionista ou profissional de saúde quando necessário.

## Próximas etapas recomendadas

1. Integrar OCR para extrair porção, calorias, açúcares e sódio de rótulos.
2. Integrar um classificador visual para reconhecer alimentos e estimar confiança.
3. Usar AR Foundation para posicionar cartões no espaço e estimar tamanho de porção.
4. Criar confirmação manual do alimento e da porção antes de calcular.
5. Persistir histórico apenas com consentimento e uma política de privacidade clara.
6. Revisar textos e limites de alerta com nutricionista responsável.

AR Foundation, ARCore e ARKit já estão declarados para facilitar a evolução. A versão atual usa `WebCamTexture` para leitura EAN-13 e ainda não ancora conteúdo no espaço por AR Foundation.

Os dados de produtos vêm da comunidade Open Food Facts e podem estar incompletos ou desatualizados. O app pede para conferir o rótulo antes de registrar. Os limites de alerta reproduzem os valores da Anvisa para rotulagem frontal e não representam diagnóstico nem avaliação personalizada. Para açúcares, só é indicado “alto em açúcar adicionado” quando a base informa especificamente açúcares adicionados; açúcar total aparece com essa distinção.

## Próximos requisitos

1. OCR no dispositivo para ler diretamente a tabela nutricional e comparar com a base.
2. Reconhecimento de alimentos sem código com confirmação do usuário e ajuste de porção.
3. Meta diária configurável com orientação profissional, além da referência geral.
4. Ancoragem de cartões no espaço via AR Foundation.
5. Histórico de macros e possibilidade de editar ou remover itens registrados.
6. Avaliação dos textos e critérios por nutricionista antes de uso clínico ou público.

## Fonte de dados

O Open Food Facts é uma base aberta mantida por contribuições comunitárias. Este projeto consulta o endpoint público de produtos por código de barras e exibe uma indicação da origem. Consulte os [termos e orientações da API](https://openfoodfacts.github.io/documentation/docs/Product-Opener/api/) antes de distribuir o app.
