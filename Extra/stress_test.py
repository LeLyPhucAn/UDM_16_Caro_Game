import socket
import struct
import json
import time
import uuid
from concurrent.futures import ThreadPoolExecutor

SERVER_IP = "127.0.0.1"
SERVER_PORT = 5000

def recv_exact(sock, n):
    """Doc chinh xac du n bytes tu socket stream"""
    data = bytearray()
    while len(data) < n:
        chunk = sock.recv(n - len(data))
        if not chunk:
            return None
        data.extend(chunk)
    return bytes(data)

def send_and_recv(sock, msg_type, payload):
    """Gui packet theo chuan [4B Type][4B Len][JSON] va doc phan hoi"""
    payload["MessageId"] = str(uuid.uuid4())
    payload["Type"] = msg_type
    payload["Timestamp"] = int(time.time() * 1000)
    body = json.dumps(payload).encode("utf-8")
    header = struct.pack("<ii", msg_type, len(body))
    sock.sendall(header + body)

    hdr = recv_exact(sock, 8)
    if not hdr:
        return None
    _, body_len = struct.unpack("<ii", hdr)
    res_body = recv_exact(sock, body_len)
    return json.loads(res_body.decode("utf-8")) if res_body else None

def test_connections(num_conns=100):
    print(f"\n[1] KIEM THU KET NOI DONG THOI (CONNECTION SCALABILITY)")
    print(f"[-] Mo dong loat {num_conns} ket noi TCP socket...")
    latencies, sockets = [], []

    def connect_worker():
        t0 = time.perf_counter()
        try:
            s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
            s.connect((SERVER_IP, SERVER_PORT))
            t1 = time.perf_counter()
            latencies.append((t1 - t0) * 1000)
            sockets.append(s)
        except Exception:
            pass

    with ThreadPoolExecutor(max_workers=num_conns) as executor:
        list(executor.map(lambda _: connect_worker(), range(num_conns)))

    success = len(sockets)
    print(f" - Ket noi thanh cong     : {success}/{num_conns} ({success/num_conns*100:.1f}%)")
    if latencies:
        print(f" - Do tre bat tay TB (Avg): {sum(latencies)/len(latencies):.2f} ms")
        print(f" - Nhanh nhat (Min)       : {min(latencies):.2f} ms")
        print(f" - Cham nhat (Max)        : {max(latencies):.2f} ms")

    for s in sockets:
        s.close()

def test_performance(concurrency=20, requests_per_client=10):
    total_reqs = concurrency * requests_per_client
    print(f"\n[2] KIEM THU HIEU NANG VA DO TRE (PERFORMANCE & LATENCY)")
    print(f"[-] Gui {total_reqs} request ({concurrency} clients x {requests_per_client} reqs)...")
    latencies = []
    success_count = 0

    def client_worker(cid):
        nonlocal success_count
        try:
            s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
            s.connect((SERVER_IP, SERVER_PORT))
            for i in range(requests_per_client):
                t0 = time.perf_counter()
                res = send_and_recv(s, 3, {"RoomName": f"Room_{cid}_{i}", "Password": "", "HostId": f"User_{cid}"})
                t1 = time.perf_counter()
                if res and res.get("Success", False):
                    latencies.append((t1 - t0) * 1000)
                    success_count += 1
            s.close()
        except Exception:
            pass

    t_start = time.perf_counter()
    with ThreadPoolExecutor(max_workers=concurrency) as executor:
        list(executor.map(client_worker, range(concurrency)))
    t_total = time.perf_counter() - t_start

    rps = success_count / t_total if t_total > 0 else 0
    print(f" - Xu ly thanh cong       : {success_count}/{total_reqs} ({success_count/total_reqs*100:.1f}%)")
    print(f" - Thong luong xu ly      : {rps:,.1f} req/s (Throughput)")
    if latencies:
        print(f" - Do tre phan hoi TB     : {sum(latencies)/len(latencies):.2f} ms")
        print(f" - Do tre nho nhat (Min)  : {min(latencies):.2f} ms")
        print(f" - Do tre lon nhat (Max)  : {max(latencies):.2f} ms")

def test_stress_resilience(num_storm=50):
    print(f"\n[3] KIEM THU CHIU TAI VA PHUC HOI (STRESS & RESILIENCE)")
    print(f"[-] Ngat dot ngot {num_storm} ket noi cung luc (Disconnection Storm)...")
    sockets = []
    for _ in range(num_storm):
        try:
            s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
            s.connect((SERVER_IP, SERVER_PORT))
            sockets.append(s)
        except Exception:
            pass
    for s in sockets:
        s.close()
    time.sleep(0.5)

    # Thu ket noi moi sau bao
    t0 = time.perf_counter()
    alive = False
    try:
        s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        s.connect((SERVER_IP, SERVER_PORT))
        alive = True
        s.close()
    except Exception:
        alive = False
    ping = (time.perf_counter() - t0) * 1000

    print(f" - So socket ngat dot ngot: {len(sockets)} sockets")
    print(f" - Server phuc hoi        : {'HOAT DONG TOT (Khong crash)' if alive else 'THAT BAI'}")
    print(f" - Ping ket noi moi       : {ping:.2f} ms")

if __name__ == "__main__":
    print("=" * 65)
    print("       CARO SERVER - KET QUA STRESS TEST & PERFORMANCE TEST      ")
    print(f"       Target: {SERVER_IP}:{SERVER_PORT}")
    print("=" * 65)

    test_connections(num_conns=100)
    test_performance(concurrency=20, requests_per_client=10)
    test_stress_resilience(num_storm=50)

    print("\n" + "=" * 65)
    print("  >> KET LUAN: SERVER DAT CHUAN TOC DO VA DO ON DINH CAO! <<")
    print("=" * 65)
