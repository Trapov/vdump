export default {
  dump: async (file: File) => {
    const formData  = new FormData();
    formData.append('dump', file, file.name);
    
    const result = await fetch('http://localhost:5000/api/v1/dumps', {
      method: 'POST',
      body: formData
    });
    if (result.status != 200){
      throw new Error((await result.json()).error);
    }
    
    const mapped = await result.json();

    return {
      id: mapped.id,
      reportItems: mapped.reportItems.map((x:any) => ({
        count: x.count,
        sizeMb: x.sizeBytes / 1024 / 1024,
        moduleName: x.moduleName,
        typeName:x.typeName,
      }))
    }
  }
}
