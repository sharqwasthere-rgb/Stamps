// QR Scanner using HTML5 Camera API and jsQR library
let videoStream = null;
let videoElement = null;
let canvasElement = null;
let canvasContext = null;
let scanInterval = null;
let dotNetRef = null;

window.startQRScanner = async function(dotNetReference) {
    dotNetRef = dotNetReference;
    
    try {
        // Wait for container to be available
        let container = document.getElementById('video-container');
        let attempts = 0;
        while (!container && attempts < 10) {
            await new Promise(resolve => setTimeout(resolve, 200));
            container = document.getElementById('video-container');
            attempts++;
        }
        
        if (!container) {
            dotNetRef.invokeMethodAsync('OnCameraError', 'Container non trovato. Riprova.');
            return;
        }
        
        // Clear previous elements
        container.innerHTML = '';
        
        videoElement = document.createElement('video');
        videoElement.setAttribute('playsinline', '');
        videoElement.setAttribute('autoplay', '');
        videoElement.style.width = '100%';
        videoElement.style.height = '100%';
        videoElement.style.objectFit = 'cover';
        container.appendChild(videoElement);
        
        // Create hidden canvas for processing
        canvasElement = document.createElement('canvas');
        canvasContext = canvasElement.getContext('2d', { willReadFrequently: true });
        
        // Request camera access
        const constraints = {
            video: {
                facingMode: 'environment',
                width: { ideal: 1280 },
                height: { ideal: 720 }
            }
        };
        
        videoStream = await navigator.mediaDevices.getUserMedia(constraints);
        videoElement.srcObject = videoStream;
        
        await videoElement.play();
        
        // Set canvas size
        canvasElement.width = videoElement.videoWidth || 640;
        canvasElement.height = videoElement.videoHeight || 480;
        
        // Start scanning
        scanInterval = setInterval(scanQRCode, 200);
        
    } catch (error) {
        console.error('Camera error:', error);
        let errorMsg = 'Impossibile accedere alla fotocamera';
        
        if (error.name === 'NotAllowedError') {
            errorMsg = 'Permesso fotocamera negato. Abilita i permessi nelle impostazioni.';
        } else if (error.name === 'NotFoundError') {
            errorMsg = 'Nessuna fotocamera trovata sul dispositivo.';
        } else if (error.name === 'NotReadableError') {
            errorMsg = 'Fotocamera già in uso da un\'altra app.';
        }
        
        if (dotNetRef) {
            dotNetRef.invokeMethodAsync('OnCameraError', errorMsg);
        }
    }
};

window.stopQRScanner = function() {
    if (scanInterval) {
        clearInterval(scanInterval);
        scanInterval = null;
    }
    
    if (videoStream) {
        videoStream.getTracks().forEach(track => track.stop());
        videoStream = null;
    }
    
    if (videoElement) {
        videoElement.srcObject = null;
    }
};

function scanQRCode() {
    if (!videoElement || !canvasElement || !canvasContext) return;
    if (videoElement.readyState !== videoElement.HAVE_ENOUGH_DATA) return;
    
    // Draw video frame to canvas
    canvasContext.drawImage(videoElement, 0, 0, canvasElement.width, canvasElement.height);
    
    // Get image data
    const imageData = canvasContext.getImageData(0, 0, canvasElement.width, canvasElement.height);
    
    // Use jsQR to decode
    if (typeof jsQR !== 'undefined') {
        const code = jsQR(imageData.data, imageData.width, imageData.height, {
            inversionAttempts: 'dontInvert'
        });
        
        if (code && code.data) {
            console.log('QR Code found:', code.data);
            
            // Stop scanning temporarily
            clearInterval(scanInterval);
            scanInterval = null;
            
            // Notify Blazor
            if (dotNetRef) {
                dotNetRef.invokeMethodAsync('OnQRCodeScanned', code.data);
            }
        }
    }
}

