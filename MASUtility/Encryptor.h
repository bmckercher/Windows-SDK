#pragma once
using namespace Windows::Storage::Streams;
using namespace Windows::Foundation;
using namespace Platform;

namespace MASUtility
{
    public ref class Encryptor sealed
    {
    public:
		IAsyncOperation<IBuffer^>^ EncryptAsync(IBuffer^ data, String^ entropy);
		IAsyncOperation<IBuffer^>^ DecryptAsync(IBuffer^ data, String^ entropy);

	private:
		BYTE* GetDataFromIBuffer(IBuffer^ buffer);
    };
}
