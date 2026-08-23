export class CustomImageUploadAdapter {
  private loader: any;

  constructor(loader: any) {
    this.loader = loader;
  }

  upload() {
    console.log('object');
    const imageUrl = 'url_to_uploaded_image';
    return new Promise<{ default: string }>((resolve) => {
      resolve({ default: imageUrl });
    });
  }

  abort() {}
}
