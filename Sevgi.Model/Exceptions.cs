using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sevgi.Model.Utilities
{
    public abstract class BaseException : Exception
    {
        public const string CODE_UNKNOWN = "UNKNOWN";

        public abstract bool IsMessageForUser { get; }

        public virtual string? TraceId
        {
            get
            {
                return null;
            }
        }

        public string Code { get; protected set; } = CODE_UNKNOWN;

        public string UserMessage
        {
            get
            {
                return GetUserMessage();
            }
        }

        public BaseException(string msg) :
            base(msg)
        {
        }

        public BaseException(Exception ex) :
            base(ex.Message, ex)
        {
        }

        public BaseException(string msg, Exception ex) :
            base(msg, ex)
        {
        }

        private string GetUserMessage()
        {
            if (IsMessageForUser)
            {
                return Message;
            }
            if (TraceId == null)
            {
                return "Bilinmeyen bir hata oluştu.";
            }
            return $"Bir hata oluştu. Hata kodu '{TraceId}' ile müşteri hizmetlerine bilgi verebilirsiniz.";
        }
    }

    public class InternalException : BaseException
    {
        public override bool IsMessageForUser
        {
            get
            {
                return false;
            }
        }

        public InternalException(string msg) :
            base(msg)
        {
        }

        public InternalException(string msg, Exception ex) :
            base(msg, ex)
        {
        }

        public InternalException(Exception ex) :
            base(ex)
        {
        }
    }

    public class UserException : BaseException
    {
        public const string AGREEMENTS_NOT_SIGNED = "AGREEMENTS_NOT_SIGNED";
        public const string USER_NOT_FOUND = "USER_NOT_FOUND";
        public const string USER_SUSPENDED = "USER_SUSPENDED";
        public const string INVALID_PASSWORD = "INVALID_PASSWORD";
        public const string INVALID_TOKEN = "INVALID_TOKEN";

        public const string USER_EXISTS = "USER_EXISTS";
        public const string MISSING_TOKEN = "MISSING_TOKEN";
        public const string INSUFFICENT_BALANCE = "INSUFFICENT_BALANCE";
        public const string SESSION_EXPIRED = "SESSION_EXPIRED";

        public override bool IsMessageForUser { get { return true; } }
        public UserException(string msg) : base(msg) { }
        public UserException(string code, string msg) : base(msg) { Code = code; }
    }

    public class UserNotFoundException : UserException
    {
        public UserNotFoundException(string msg = "Kullanıcı bulunamadı.") : base(USER_NOT_FOUND, msg) { }
    }
    public class UserExistsException : UserException
    {
        public UserExistsException(string msg = "Kullanıcı zaten kayıtlı.") : base(USER_EXISTS, msg) { }
    }

    public class InvalidPasswordException : UserException
    {
        public InvalidPasswordException(string msg = "Hatalı parola.") : base(INVALID_PASSWORD, msg) { }
    }


    public class MembershipSuspendedException : UserException
    {
        public MembershipSuspendedException(string msg = "Üyeliğiniz askıya alınmış.") : base(USER_SUSPENDED, msg) { }
    }

    public class MissingTokenException : UserException
    {
        public MissingTokenException(string msg = "Token bilgileri eksik.") : base(MISSING_TOKEN, msg) { }
    }

    public class InsufficientBalanceException : UserException
    {
        public InsufficientBalanceException(string msg = "Bu işlem için yeterli bakiyeniz bulunmamaktadır.") : base(INSUFFICENT_BALANCE, msg) { }
    }

    public class SessionExpiredException : UserException
    {
        public SessionExpiredException(string msg = "Oturumunuz zaman aşımına uğradı") : base(SESSION_EXPIRED, msg) { }
    }

    public class InvalidTokenException : UserException
    {
        public InvalidTokenException(string msg = "Hatalı parola.") : base(INVALID_PASSWORD, msg) { }
    }
}
