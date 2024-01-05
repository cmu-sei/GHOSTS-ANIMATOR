# Content Models

This repository contains modelfiles intended for building customized models on ollama.ai. These models enhance various facets of digital interactions and content creation:

1. **Activity**: Generates simulations of agent activity, encompassing a wide range of behaviors and scenarios.
2. **Chat**: Tailors responses and interactions for agents using chat platforms or posting messages on social media, ensuring relevance and engagement.
3. **Content**: Facilitates the creation of documents and other artifacts, streamlining the process of generating authentic and contextually appropriate content.

## Getting Started

```bash
cd activity
ollama create activity
ollama run activity
```

To tear down the model, run:

```bash
rm ~/.ollama/history
ollama rm activity
```

And then rebuild.