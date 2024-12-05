using System.Collections.Concurrent;
using UnityEngine;
using WebSocketSharp;

namespace WebRTCTutorial
{
    // 메시지를 받았을 때 호출될 델리게이트 정의
    public delegate void MessageHandler(string message);

    public class WebSocketClient : MonoBehaviour
    {
        // 메시지를 받았을 때 호출되는 이벤트 정의
        public event MessageHandler MessageReceived;

        // WebSocket 서버에 메시지를 전송하는 메서드
        public void SendWebSocketMessage(string message) => _ws.Send(message);

        // Unity의 Awake 메서드 - 초기화 작업
        protected void Awake()
        {
            // 서버 IP가 설정되지 않은 경우 'localhost'로 기본값 설정
            var ip = string.IsNullOrEmpty(_serverIp) ? "localhost" : _serverIp;

            // WebSocket URL 구성 (예: ws://localhost:8080)
            var url = $"ws://{ip}:8080";

            // WebSocket 객체 생성
            _ws = new WebSocket(url);

            // 메시지 수신 이벤트와 에러 이벤트에 대한 핸들러 연결
            _ws.OnMessage += OnMessage;
            _ws.OnError += OnError;

            // 서버에 연결
            _ws.Connect();
            // 모바일에서 연결이 잘 되는지 디버그 로그 추가
            Debug.Log("Attempting to connect to WebSocket server at " + url);
        }

        // Unity의 Update 메서드 - 매 프레임마다 호출
        protected void Update()
        {
            // 큐에 저장된 에러 메시지를 메인 스레드에서 처리
            while (_receivedErrors.TryDequeue(out var error))
            {
                Debug.LogError("WS error: " + error); // 에러가 발생하면 출력
            }

            // 큐에 저장된 메시지를 메인 스레드에서 처리
            while (_receivedMessages.TryDequeue(out var message))
            {
                Debug.Log("WS Message Received: " + message); // 메시지가 수신되면 출력
                MessageReceived?.Invoke(message); // 이벤트 발생
            }
        }

        // Unity의 OnDestroy 메서드 - 객체가 파괴될 때 호출
        protected void OnDestroy()
        {
            // WebSocket 객체가 null인 경우 종료
            if (_ws == null)
            {
                return;
            }

            // 이벤트 핸들러 해제
            _ws.OnMessage -= OnMessage;
            _ws.OnError -= OnError;

            // WebSocket 연결 종료
            _ws.Close();
            _ws = null;
        }

        // 서버 IP를 설정할 수 있는 변수 (Inspector에서 설정 가능)
        [SerializeField]
        private string _serverIp;

        // WebSocket 객체
        private WebSocket _ws;

        // 메시지와 에러를 큐에 저장하여 Update에서 처리
        private readonly ConcurrentQueue<string> _receivedMessages = new ConcurrentQueue<string>();
        private readonly ConcurrentQueue<string> _receivedErrors = new ConcurrentQueue<string>();

        // 메시지 수신 시 호출되는 이벤트 핸들러
        private void OnMessage(object sender, MessageEventArgs e)
        {
            // 받은 메시지를 큐에 저장
            _receivedMessages.Enqueue(e.Data);
        }

        // 에러 발생 시 호출되는 이벤트 핸들러
        private void OnError(object sender, ErrorEventArgs e)
        {
            // 받은 에러 메시지를 큐에 저장
            _receivedErrors.Enqueue(e.Message);
        }
    }
}
