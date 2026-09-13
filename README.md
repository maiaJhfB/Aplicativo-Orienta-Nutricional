# NutriAR — orientação nutricional em realidade aumentada

Protótipo inicial para **Unity 6000.0.0f1**, pensado primeiro para celular em orientação vertical. O app mostra a câmera, simula a identificação de alimentos ou rótulos e apresenta uma estimativa de calorias, macronutrientes, alertas e uma orientação breve.

## O que já funciona

- Câmera traseira do celular ou webcam do computador como fundo da experiência.
- Moldura e animação de escaneamento.
- Cinco perfis nutricionais de demonstração: banana, refrigerante, barra de cereal, pão de queijo e iogurte.
- Estimativa de calorias e percentual de uma referência diária de 2.000 kcal.
- Barras de carboidratos, proteínas e gorduras.
- Alertas em três níveis: positivo, atenção e alto.
- Layout vertical adaptado à área segura de aparelhos com recorte de tela.
- Funcionamento no Editor mesmo sem câmera, por meio dos botões de teste rápido.

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

AR Foundation, ARCore e ARKit já estão declarados no projeto para facilitar a evolução. A versão atual usa `WebCamTexture` de propósito: ela permite validar a experiência de câmera no Editor e em aparelhos sem exigir uma cena AR específica.

