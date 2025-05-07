import pytest
import sys

def run_tests():
    args = [
        "-v",
        "--cov=src",
        "--cov-report=html",
        "-m", "not integration"  # Skip integration tests by default
    ]
    return pytest.main(args)

if __name__ == "__main__":
    sys.exit(run_tests())