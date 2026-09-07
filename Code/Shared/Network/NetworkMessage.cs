using System;

namespace Shared.Network
{
    /// <summary>
    /// Cấu trúc dữ liệu chuẩn hóa cho 1 gói tin ở tầng network, tách biệt
    /// khỏi logic đóng gói/giải mã (nằm trong PacketParser):
    /// [4 byte Type] [4 byte độ dài Body] [Body JSON dạng UTF-8]
    /// </summary>
    public class NetworkMessage
    {
        public const int TYPE_SIZE = 4;
        public const int LENGTH_SIZE = 4;
        public const int HEADER_SIZE = TYPE_SIZE + LENGTH_SIZE;

        /// <summary>Giới hạn Body tối đa (1 MB) để tránh packet giả mạo khai báo độ dài quá lớn.</summary>
        public const int MAX_BODY_SIZE = 1024 * 1024;

        /// <summary>Giá trị số nguyên của MessageType, đọc trực tiếp từ Header.</summary>
        public int RawType { get; set; }

        /// <summary>Độ dài Body (byte), đọc trực tiếp từ Header.</summary>
        public int BodyLength { get; set; }

        /// <summary>Nội dung Body dạng JSON (UTF-8).</summary>
        public byte[] Body { get; set; }
    }

    /// <summary>
    /// Phân loại lỗi packet, dùng để MessageHandler biết chính xác cần xử lý
    /// theo hướng nào:
    /// - MissingData          -> "Xử lý Packet thiếu dữ liệu"
    /// - InvalidMessageType   -> "Xử lý MessageType không hợp lệ"
    /// - CorruptedPacket      -> "Xử lý Packet bị lỗi" (JSON hỏng, thiếu
    ///   MessageId, hoặc deserialize thất bại)
    /// </summary>
    public enum PacketErrorType
    {
        MissingData,
        InvalidMessageType,
        CorruptedPacket
    }

    /// <summary>
    /// Exception riêng cho lỗi đóng gói/giải mã packet. Có thêm ErrorType để
    /// MessageHandler không cần đọc chuỗi Message mà vẫn biết chính xác đây
    /// là loại lỗi nào trong 3 loại cần xử lý riêng.
    /// </summary>
    public class PacketException : Exception
    {
        public PacketErrorType ErrorType { get; }

        public PacketException(PacketErrorType errorType, string message) : base(message)
        {
            ErrorType = errorType;
        }

        public PacketException(PacketErrorType errorType, string message, Exception innerException)
            : base(message, innerException)
        {
            ErrorType = errorType;
        }
    }
}
