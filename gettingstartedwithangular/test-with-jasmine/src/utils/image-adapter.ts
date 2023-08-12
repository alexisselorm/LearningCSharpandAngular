import { Editor } from '@ckeditor/ckeditor5-core';
import ImageUploadAdapter from '@ckeditor/ckeditor5-image/src/imageupload';

export class CustomImageUploadAdapter extends ImageUploadAdapter {
  constructor(loader: Editor) {
    super(loader);
  }

  upload() {
    // Implement your image upload logic here.
    console.log('object');
    // This could involve sending the file to a server and returning the image URL.
    const imageUrl = 'url_to_uploaded_image'; // Replace with the actual URL
    return new Promise((resolve, reject) => {
      resolve({ default: imageUrl });
    });
  }
}

interface ImageUploadAdapterLoader {
  upload(file: File): Promise<{ default: string }>;
}
