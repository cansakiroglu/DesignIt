from ..sketchfab_connector import SketchFabConnector

def test_search():
    connector = SketchFabConnector("0d0c5741ed93477986ae00986540961b")
    result = connector.search("cat")
    assert result is not None
    assert result["total"] > 0
    assert result["results"][0]["name"] == "Cat"

def test_download():
    connector = SketchFabConnector("0d0c5741ed93477986ae00986540961b")
    result = connector.search("cat")
    assert result is not None
    assert result["total"] > 0
    model_id = result["results"][0]["uid"]
    data = connector.download(model_id)
    assert data is not None
    assert len(data) > 0