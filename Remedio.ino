/*
  Projeto: medição local e via mqtt
  Autor: 
*/
#include <WiFi.h>
#include <Wire.h>
#include <PubSubClient.h>
#include <HTTPClient.h>
#include <Arduino_JSON.h>


/*
   Defines do projeto
*/
/* GPIO do módulo WiFi ESP32 que o pino de comunicação do sensor está ligado. */
#define SENSOR_SUNDAY 15
#define SENSOR_MONDAY 2
#define SENSOR_TUESDAY 4
#define SENSOR_WEDNESDAY 5
#define SENSOR_THURSDAY 18
#define SENSOR_FRIDAY 19
#define SENSOR_SATURDAY 21

/* defines de id mqtt e tópicos para publicação e subscribe */
#define TOPICO_PUBLISH   "MalandrimMQTTRemedios"    /*tópico MQTT de envio de informações para Broker
                                                 IMPORTANTE: recomenda-se fortemente alterar os nomes
                                                             desses tópicos. Caso contrário, há grandes
                                                             chances de você controlar e monitorar o módulo
                                                             de outra pessoa (pois o broker utilizado contém 
                                                             dispositivos do mundo todo). Altere-o para algo 
                                                             o mais único possível para você. */

#define ID_MQTT  "MalandrimESP320001"     /* id mqtt (para identificação de sessão)
                                       IMPORTANTE: este deve ser único no broker (ou seja, 
                                                   se um client MQTT tentar entrar com o mesmo 
                                                   id de outro já conectado ao broker, o broker 
                                                   irá fechar a conexão de um deles). Pelo fato
                                                   do broker utilizado conter  dispositivos do mundo 
                                                   todo, recomenda-se fortemente que seja alterado 
                                                   para algo o mais único possível para você.*/


/* Constantes */
const char* SSID = "MALANDRIM"; // coloque aqui o SSID / nome da rede WI-FI que deseja se conectar
const char* PASSWORD = "malandrim123456"; // coloque aqui a senha da rede WI-FI que deseja se conectar

const char* BROKER_MQTT = "broker.emqx.io"; //URL do broker MQTT que se deseja utilizar
const int BROKER_PORT = 1883; // Porta do Broker MQTT

const char* serverName = "http://localhost:8080/remedios";

/********* Variáveis e objetos globais ********/
WiFiClient espClient; // Cria o objeto espClient
PubSubClient MQTT(espClient); // Instancia o Cliente MQTT passando o objeto espClient

int tem_remedio_domingo = 0;
int tem_remedio_segunda = 0;
int tem_remedio_terca = 0;
int tem_remedio_quarta = 0;
int tem_remedio_quinta = 0;
int tem_remedio_sexta = 0;
int tem_remedio_sabado = 0;
int teve_alteracao = 0;

char mensagem[200];

void init_wifi(void)
{
  delay(10);
  Serial.println("------Conexao WI-FI------");
  Serial.print("Conectando-se na rede: ");
  Serial.println(SSID);
  Serial.println("Aguarde");
  reconnect_wifi();
}

void init_MQTT(void)
{
  //informa qual broker e porta deve ser conectado
  MQTT.setServer(BROKER_MQTT, BROKER_PORT);
}

void reconnect_MQTT(void)
{
  if (MQTT.connected())
    return;
  
  while (!MQTT.connected())
  {
    Serial.print("* Tentando se conectar ao Broker MQTT: ");
    Serial.println(BROKER_MQTT);
    if (MQTT.connect(ID_MQTT)) {
      Serial.println("Conectado com sucesso ao broker MQTT!");
    }
    else {
      Serial.println("Falha ao reconectar no broker.");
      Serial.println("Havera nova tentativa de conexao em 2s");
    }
  }
}

void reconnect_wifi(void) {
  
  if (WiFi.status() == WL_CONNECTED)
    return;

  WiFi.begin(SSID, PASSWORD); // Conecta na rede WI-FI

  Serial.print("Tentando conectar na rede");

  while (WiFi.status() != WL_CONNECTED)
  {
    delay(500);
    Serial.print(".");
  }

  Serial.println();
  Serial.print("Conectado com sucesso na rede ");
  Serial.print(SSID);
  Serial.println("IP obtido: ");
  Serial.println(WiFi.localIP());
}

void verifica_conexoes_wifi_e_MQTT(void) {

  reconnect_wifi();

  reconnect_MQTT();
}

void envia_medicoes_para_serial()
{
  char i;

  /* pula 80 linhas, de forma que no monitor serial seja exibida somente as mensagens atuais (impressao de refresh de tela) */
  for (i = 0; i < 80; i++)
    Serial.println(" ");

  /* constrói mensagens e as envia */
  memset(mensagem, 0, sizeof(mensagem));
  sprintf(mensagem, "DOM=%d SEG=%d TER=%d QUA=%d QUI=%d SEX=%d SAB=%d ", tem_remedio_domingo, tem_remedio_segunda, 
                                                                         tem_remedio_terca, tem_remedio_quarta, 
                                                                         tem_remedio_quinta, tem_remedio_sexta, tem_remedio_sabado);
  Serial.println(mensagem);

}

/*
   Função: envia por MQTT as informações de temperatura e umidade lidas, assim como as temperaturas máxima e mínima
   Parâmetros: - Temperatura lida
               - Umidade relativa do ar lida
   Retorno: nenhum
*/
void envia_informacoes_por_mqtt() {

  MQTT.publish(TOPICO_PUBLISH, mensagem);
}

void setup() {
  /* configura comunicação serial (para enviar mensgens com as medições)
    e inicializa comunicação com o sensor.
  */
  Serial.begin(115200);
  pinMode(SENSOR_SUNDAY, INPUT);
  pinMode(SENSOR_MONDAY, INPUT);
  pinMode(SENSOR_TUESDAY, INPUT);
  pinMode(SENSOR_WEDNESDAY, INPUT);
  pinMode(SENSOR_THURSDAY, INPUT);
  pinMode(SENSOR_FRIDAY, INPUT);
  pinMode(SENSOR_SATURDAY, INPUT);
  
  /* inicializações do WI-FI e MQTT */
  init_wifi();
  init_MQTT();
}

/*
   Programa principal
*/

void loop() {

  // verifica se tem remedio nas caixinhas
  remedio_domingo();
  remedio_segunda();
  remedio_terca();
  remedio_quarta();
  remedio_quinta();
  remedio_sexta();
  remedio_sabado();

  /* Verifica se as conexões MQTT e wi-fi estão ativas
     Se alguma delas não estiver ativa, a reconexão é feita */
  verifica_conexoes_wifi_e_MQTT();

  if (teve_alteracao == 1){
    Serial.println("TEVE MUDANCA DE REMEDIO");
    delay(2000);
    envia_medicoes_para_serial();

    envia_informacoes_por_mqtt();
  
    envia_informacoes_por_http();
  }
   
  /* Faz o keep-alive do MQTT */
  MQTT.loop();

  teve_alteracao = 0;
  /* espera cinco segundos até a próxima leitura  */
  delay(10000);
}


void envia_informacoes_por_http(){
           
  String msg = "GET /api/remedioapi/EnviarInformacaoCaixa?pacienteId=1&dom=";
         msg += tem_remedio_domingo; 
         msg += "&seg=";
         msg += tem_remedio_segunda; 
         msg += "&ter=";
         msg += tem_remedio_terca; 
         msg += "&qua=";
         msg += tem_remedio_quarta; 
         msg += "&qui=";
         msg += tem_remedio_quinta; 
         msg += "&sex=";
         msg += tem_remedio_sexta; 
         msg += "&sab=";
         msg += tem_remedio_sabado; 
         msg += " HTTP/1.1\r\nHost: remediounisal.wsilveirait.com.br\r\nConnection: close\r\n\r\n";

  //envia a mensagem     
  if (espClient.connect("remediounisal.wsilveirait.com.br", 80)) {
    Serial.println("connected");
    // Make a HTTP request:
    espClient.println(msg);
    espClient.println();
  }

}

void remedio_domingo(){
    
    if (digitalRead(SENSOR_SUNDAY) == LOW){
      if (tem_remedio_domingo == 0) {
        teve_alteracao = 1;
      }
      tem_remedio_domingo = 1;
    }else{
      if (tem_remedio_domingo == 1) {
        teve_alteracao = 1;
      }
      tem_remedio_domingo = 0;
    }
}

void remedio_segunda(){
    
    if (digitalRead(SENSOR_MONDAY) == LOW){
      if (tem_remedio_segunda == 0) {
        teve_alteracao = 1;
      }
      tem_remedio_segunda = 1;
    }else{
      if (tem_remedio_segunda == 1) {
        teve_alteracao = 1;
      }      
      tem_remedio_segunda = 0;
    }
}

void remedio_terca(){
    
    if (digitalRead(SENSOR_TUESDAY) == LOW){
      if (tem_remedio_terca == 0) {
        teve_alteracao = 1;
      }
      tem_remedio_terca = 1;
    }else{
      if (tem_remedio_terca == 1) {
        teve_alteracao = 1;
      }      
      tem_remedio_terca = 0;
    }
}

void remedio_quarta(){
    
    if (digitalRead(SENSOR_WEDNESDAY) == LOW){
      if (tem_remedio_quarta == 0) {
        teve_alteracao = 1;
      }      
      tem_remedio_quarta = 1;
    }else{
      if (tem_remedio_quarta == 1) {
        teve_alteracao = 1;
      }      
      tem_remedio_quarta = 0;
    }
}

void remedio_quinta(){
    
    if (digitalRead(SENSOR_THURSDAY) == LOW){
      if (tem_remedio_quinta == 0) {
        teve_alteracao = 1;
      }      
      tem_remedio_quinta = 1;
    }else{
      if (tem_remedio_quinta == 1) {
        teve_alteracao = 1;
      }      
      tem_remedio_quinta = 0;
    }
}

void remedio_sexta(){
    
    if (digitalRead(SENSOR_FRIDAY) == LOW){
      if (tem_remedio_sexta == 0) {
        teve_alteracao = 1;
      }      
      tem_remedio_sexta = 1;
    }else{
      if (tem_remedio_sexta == 1) {
        teve_alteracao = 1;
      }      
      tem_remedio_sexta = 0;
    }
}

void remedio_sabado(){
    
    if (digitalRead(SENSOR_SATURDAY) == LOW){
      if (tem_remedio_sabado == 0) {
        teve_alteracao = 1;
      }      
      tem_remedio_sabado = 1;
    }else{
      if (tem_remedio_sabado == 1) {
        teve_alteracao = 1;
      }      
      tem_remedio_sabado = 0;
    }
}
