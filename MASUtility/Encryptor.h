/*
Copyright (c) 2016 CA. All rights reserved.
This software may be modified and distributed under the terms
of the MIT license. See the LICENSE file for details.
*/

﻿#pragma once
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
