#!/bin/bash
# Test Docker build script for Linux/macOS

echo "=========================================="
echo "  Testing LoynovaGrandNode2 Docker Build"
echo "=========================================="

echo "Testing alternative Dockerfile..."
if docker build -f Dockerfile.alternative -t loynovagrandnode2:test .; then
    echo ""
    echo "✅ Alternative Dockerfile build succeeded!"
    echo "Use: docker-compose -f docker-compose.alternative.yml up -d"
else
    echo ""
    echo "❌ Alternative Dockerfile build failed!"
    echo "Trying original Dockerfile..."
    echo ""
    
    if docker build -f Dockerfile -t loynovagrandnode2:test .; then
        echo ""
        echo "✅ Original Dockerfile build succeeded!"
        echo "Use: docker-compose up -d"
    else
        echo ""
        echo "❌ Original Dockerfile also failed!"
        echo "Please check the build logs above for errors."
        exit 1
    fi
fi

echo ""
echo "🎉 Build test completed successfully!"
echo "You can now run the full stack with docker-compose."