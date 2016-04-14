#pragma once
using namespace Windows::Storage::Streams;
using namespace Windows::Foundation;
using namespace Platform;

namespace MASUtility
{
    public ref class Encryptor sealed
    {
    public:
		IAsyncOperation<IBuffer^>^ EncryptAsync(IBuffer^ data);
		IAsyncOperation<IBuffer^>^ DecryptAsync(IBuffer^ data);

	private:
		BYTE* GetDataFromIBuffer(IBuffer^ buffer);
    };
}
