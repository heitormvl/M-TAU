# Persona 01: Cliente com Deficiência Motora

## 1. Identificação
* **Nome:** Ricardo Oliveira
* **Idade:** 42 anos
* **Ocupação:** Engenheiro Civil (Atualmente em readaptação profissional)
* **Localização:** São Paulo, SP
* **Nível Tecnológico:** Avançado
* **Citação:** "Busco equipamentos de alta performance com custo reduzido para manter minha autonomia diária."

## 2. Contexto de Saúde e Acessibilidade
* **Condição:** Paraplegia decorrente de lesão medular (T12).
* **Tecnologias Assistivas Atuais:** Cadeira de rodas monobloco, almofada antiescaras de alta tecnologia e comandos adaptados no veículo.
* **Limitações de Interação:** Possui destreza manual preservada, porém apresenta fadiga muscular rápida. Prefere navegação via teclado ou dispositivos apontadores com alta precisão (trackball).

## 3. Objetivos e Motivações
* Encontrar peças de reposição específicas para sua cadeira de rodas atual (rodas de fibra de carbono ou eixos).
* Adquirir uma cadeira de rodas motorizada seminova para trajetos longos, dado o alto custo de um equipamento novo.
* Validar a procedência técnica do equipamento através de laudos ou fotos detalhadas.

## 4. Pontos de Dor
* Dificuldade em encontrar filtros específicos por medidas (largura do assento, cambagem) em marketplaces genéricos.
* Interfaces com áreas de clique (hit targets) muito pequenas que exigem esforço motor excessivo.
* Falta de informações sobre o tempo de uso e estado de conservação da bateria em itens motorizados.

## 5. Impacto no Design e Desenvolvimento
* **Requisitos Relacionados:** RF03 (Filtros Especializados) e RF04 (Especificações Técnicas).
* **Implementação Técnica:** Garantir que todos os elementos interativos tenham no mínimo 44x44 pixels e que o fluxo de checkout seja simplificado para evitar cliques repetitivos.

# Persona 02: Cliente com Deficiência Visual

## 1. Identificação
* **Nome:** Ana Beatriz
* **Idade:** 26 anos
* **Ocupação:** Estudante de Pedagogia
* **Localização:** Curitiba, PR
* **Nível Tecnológico:** Intermediário
* **Citação:** "A interface do sistema deve ser invisível; o que importa é a clareza da informação para quem não enxerga."

## 2. Contexto de Saúde e Acessibilidade
* **Condição:** Cegueira congênita (Amaurose).
* **Tecnologias Assistivas Atuais:** Leitor de tela NVDA (Windows), TalkBack (Android) e linha Braille.
* **Limitações de Interação:** Dependência total de feedback sonoro e hierarquia lógica de cabeçalhos. Não utiliza mouse.

## 3. Objetivos e Motivações
* Comprar um dispositivo de leitura autônoma (OrCam) ou teclados adaptados para seus estudos.
* Navegar de forma independente pelo catálogo sem auxílio de terceiros.
* Utilizar o chat interno para negociar o frete com o vendedor.

## 4. Pontos de Dor
* Imagens de produtos sem descrição textual (Alt Text), impedindo a compreensão do estado do item.
* Formulários de cadastro que não anunciam erros de validação para o leitor de tela.
* Captchas visuais ou elementos de interface que não recebem foco via teclado (TAB).

## 5. Impacto no Design e Desenvolvimento
* **Requisitos Relacionados:** RNF01 (Compatibilidade com Leitores de Tela) e RF05 (Chat em Tempo Real).
* **Implementação Técnica:** Utilização rigorosa de ARIA Labels, tags semânticas (HTML5) e notificações de Live Regions para mensagens recebidas no chat.

# Persona 03: Vendedor (Gestor de Clínica)

## 1. Identificação
* **Nome:** Marcos Vinícius
* **Idade:** 50 anos
* **Ocupação:** Gestor Administrativo de Clínica de Reabilitação
* **Localização:** Belo Horizonte, MG
* **Nível Tecnológico:** Básico/Intermediário
* **Citação:** "Precisamos de um canal oficial para repassar equipamentos seminovos que não são mais utilizados na clínica, garantindo que cheguem a quem precisa."

## 2. Contexto de Negócio
* **Perfil:** Pessoa Jurídica que realiza a renovação anual de inventário.
* **Equipamentos Disponíveis:** Andadores, guinchos de transferência, estabilizadores verticais e muletas.
* **Limitações de Interação:** Utiliza o sistema principalmente via desktop durante o horário comercial. Possui pouco tempo para cadastros complexos.

## 3. Objetivos e Motivações
* Escoar o estoque de equipamentos usados de forma rápida e segura.
* Garantir que as especificações técnicas (peso suportado, altura regulável) estejam claras para evitar devoluções.
* Centralizar a comunicação com potenciais compradores em uma plataforma rastreável.

## 4. Pontos de Dor
* Perda de tempo respondendo perguntas básicas que poderiam estar detalhadas no anúncio.
* Insegurança em relação ao recebimento do pagamento em vendas para outros estados.
* Dificuldade em gerenciar múltiplos anúncios simultaneamente em plataformas não especializadas.

## 5. Impacto no Design e Desenvolvimento
* **Requisitos Relacionados:** RF02 (Catálogo de Produtos) e RF09 (Gestão de Inventário).
* **Implementação Técnica:** Desenvolvimento de um Dashboard de Vendas simplificado e um formulário de cadastro de produto baseado em templates técnicos por categoria, automatizando a inserção de dados obrigatórios.